using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Core
{
    public class BallsController : MonoBehaviour
    {
        public PathCreator pathCreator;
        public BallSpawner spawner;
        public float speed = 1f;
        public float offsetBeetweenBalls = 0.36f;
        List<PathFollower> ballsChain;
        string tagBall = "Chain";

        public bool spawnEnable = true;
        public bool isMove = true;

        void Start()
        {
            ballsChain = new List<PathFollower>();

            StartCoroutine(ProcessAddingBalls());
        }

        public void AddBallToChain()
        {
            int ballsCount = ballsChain.Count;
            float distance = ballsCount > 0 ? ballsChain[--ballsCount].Distance : 0;
            distance -= offsetBeetweenBalls;

            PathFollower ball = spawner.SpawnBall();
            ball.Initialize(pathCreator, distance);
            ball.tag = tagBall;
            ballsChain.Add(ball);
        }

        // вставляет шар в цепь относительно шара с которым столкнулся
        public void InsertBallToChain(PathFollower ball, PathFollower coll)
        {
            if (ball == null || coll == null) {
                Debug.Log("Error: Collision happened but some object is null");
                return;
            }

            /*test*/
            ball.GetComponent<Ball>().SetColor(Color.blue); coll.GetComponent<Ball>().SetColor(Color.green);

            int collIndex = ballsChain.FindIndex(x => x.id == coll.id);
            int frontCollIndex = collIndex > 0 ? collIndex - 1 : 0;

            float collDistance = Vector2.Distance(ball.transform.position, ballsChain[collIndex].transform.position);
            float frontCollDistance = Vector2.Distance(ball.transform.position, ballsChain[frontCollIndex].transform.position);
            float cosinus = (frontCollDistance * frontCollDistance - collDistance * collDistance - offsetBeetweenBalls * offsetBeetweenBalls) 
                / (-2 * collDistance * offsetBeetweenBalls);        // теорема косинусов

            int nextInsertBallIndex;
            if(cosinus >= 0) {
                nextInsertBallIndex = frontCollIndex;
            }
            else {
                nextInsertBallIndex = collIndex;
            }

            if (collDistance == -1) {
                Debug.Log("Error: ball is collided with something unexpected and that exactly isn't chain of balls\n" +
                    "Distance = " + collDistance);
                isMove = false;
            }

            ball.tag = tagBall;
            ball.Initialize(pathCreator, ballsChain[nextInsertBallIndex].Distance);
            Shift(nextInsertBallIndex);
            if (nextInsertBallIndex == ballsChain.Count) {
                ballsChain.Add(ball);
            }
            else {
                ballsChain.Insert(nextInsertBallIndex + 1, ball);
            }
        }

        void Shift(int index)
        {
            for(int i = 0; i <= index; i++) {
                ballsChain[i].Distance += offsetBeetweenBalls;
            }
        }

        IEnumerator ProcessAddingBalls()
        {
            while (true) {
                if (spawnEnable) {
                    AddBallToChain();
                }
                yield return new WaitForSeconds(offsetBeetweenBalls / speed);        // will optimize!
            }
        }

        #region Movement

        void FixedUpdate()
        {
            if(isMove)
                MoveAllBalls(Time.fixedDeltaTime * speed);
        }

        void MoveAllBalls(float delta)
        {
            for (int i = 0; i < ballsChain.Count; i++) {
                ballsChain[i].MoveUpdate(delta);
            }
        }
        #endregion

        #region DebugGizmoz

        void OnDrawGizmos()
        {
            
        }
        #endregion
    }
}
