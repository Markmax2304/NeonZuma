using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Core.Generator;
using Core.Pool;

namespace Core
{
    public class BallsController : MonoBehaviour
    {
        public PathCreator pathCreator;
        public PoolObjectKeeper poolKeeper;
        public BallRandom randomizator;

        [Space]
        public Vector2 spawnPosition;
        public float speed = 1f;
        public float offsetBeetweenBalls = 0.36f;
        public float timeForInsertingAnimation = 3f;

        List<PathFollower> ballsList;
        /*[SerializeField]*/ BallCountRecords records;
        string tagBall = "Chain";

        [Space]
        public bool spawnEnable = true;
        public bool isMove = true;

        int countElementToSpawn = 0;
        BallInfo ballTypeToSpawn;

        void Start()
        {
            records = new BallCountRecords();
            ballsList = new List<PathFollower>();
            poolKeeper = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
            randomizator = GetComponent<BallRandom>();

            StartCoroutine(ProcessAddingBalls());
        }

        #region Ball operation in Chain

        public void AddBallToEndOfChain()
        {
            if(countElementToSpawn == 0) {
                ballTypeToSpawn = randomizator.GetRandomBallSequence(out countElementToSpawn);
                countElementToSpawn--;
            }
            else {
                countElementToSpawn--;
            }

            int ballsCount = ballsList.Count;
            float distance = ballsCount > 0 ? ballsList[--ballsCount].Distance : 0;
            distance -= offsetBeetweenBalls;

            PathFollower ball = poolKeeper.RealeseObject(spawnPosition).GetComponent<PathFollower>();
            ball.GetComponent<Ball>().Initialize(ballTypeToSpawn);
            ball.Initialize(pathCreator, distance);
            ball.tag = tagBall;
            ballsList.Add(ball);
            records.AddBall(ballTypeToSpawn.type);
        }

        // вставляет шар в цепь относительно шара с которым столкнулся
        public void InsertBallToChain(PathFollower ball, PathFollower coll)
        {
            if (ball == null || coll == null) {
                Debug.Log("Error: Collision happened but some object is null");
                return;
            }

            int collIndex = ballsList.FindIndex(x => x.id == coll.id);
            int frontBallIndex = 0, backBallIndex = 0;
            // алгоритм не оптимальный, но рабочий
            // столкновение с первым шаром рассматриваем как особый случай
            if(collIndex == 0) {
                // теорема косинусов
                float collDistance = Vector2.Distance(ball.transform.position, ballsList[collIndex].transform.position);
                float backCollDistance = Vector2.Distance(ball.transform.position, ballsList[collIndex + 1].transform.position);
                float cosinus = (backCollDistance * backCollDistance - collDistance * collDistance - offsetBeetweenBalls * offsetBeetweenBalls)
                    / (-2 * collDistance * offsetBeetweenBalls);

                if (cosinus >= 0) {
                    frontBallIndex = 0;
                    backBallIndex = frontBallIndex + 1;
                }
                else {
                    frontBallIndex = -1;
                    backBallIndex = frontBallIndex + 1;
                }
            }
            else if(collIndex > 0) {

                if (collIndex == -1) {
                    Debug.Log("Error: ball is collided with something unexpected and that exactly isn't chain of balls\n" +
                        "Distance = " + collIndex);
                    isMove = false;
                    return;
                }

                // теорема косинусов
                float collDistance = Vector2.Distance(ball.transform.position, ballsList[collIndex].transform.position);
                float frontCollDistance = Vector2.Distance(ball.transform.position, ballsList[collIndex - 1].transform.position);
                float cosinus = (frontCollDistance * frontCollDistance - collDistance * collDistance - offsetBeetweenBalls * offsetBeetweenBalls)
                    / (-2 * collDistance * offsetBeetweenBalls);        

                if (cosinus >= 0) {
                    frontBallIndex = collIndex - 1;
                    backBallIndex = frontBallIndex + 1;
                }
                else {
                    frontBallIndex = collIndex;
                    backBallIndex = frontBallIndex + 1;
                }
            }

            float distance = frontBallIndex == -1 ? ballsList[0].Distance + offsetBeetweenBalls : ballsList[frontBallIndex].Distance;
            ball.Initialize(pathCreator, distance);
            ball.tag = tagBall;
            ShiftBallsOnInserting(ball, frontBallIndex);
            if (frontBallIndex == ballsList.Count) {
                ballsList.Add(ball);
            }
            else {
                ballsList.Insert(backBallIndex, ball);
            }
            records.AddBall(ball.GetComponent<Ball>().Type);

            //проверка на цвет шаров и возможное уничтожение
        }

        IEnumerator ProcessAddingBalls()
        {
            while (true) {          // переделать спаун шаров, чтоб он зависел от тригерра коллайдера спаун объекта (последний шар вышел из него - создаём новый)
                if (spawnEnable && isMove) {
                    AddBallToEndOfChain();
                }
                yield return new WaitForSeconds(offsetBeetweenBalls / speed);        // will optimize!
            }
        }

        public BallType GetNextBallType()
        {
            int index = Random.Range(0, records.balls.Count);
            return records.balls[index].type;
        }
        #endregion

        #region Movement

        void FixedUpdate()
        {
            if(isMove)
                MoveAllBalls(Time.fixedDeltaTime * speed);
        }

        void MoveAllBalls(float delta)
        {
            for (int i = 0; i < ballsList.Count; i++) {
                ballsList[i].MoveUpdate(delta);
            }
        }

        void ShiftBallsOnInserting(PathFollower newBall, int index)
        {
            isMove = false;

            for (int i = 0; i <= index; i++) {
                ballsList[i].MoveDistance(offsetBeetweenBalls, timeForInsertingAnimation);
            }
            newBall.MoveDistance(0, timeForInsertingAnimation, delegate () { isMove = true; });
        }
        #endregion

        #region DebugGizmoz

        void OnDrawGizmos()
        {
            
        }
        #endregion
    }

    [System.Serializable]
    public class BallCountRecords
    {
        public List<CountBall> balls;

        public BallCountRecords()
        {
            balls = new List<CountBall>();
        }

        public void AddBall(BallType type)
        {
            int index = balls.FindIndex(x => x.type == type);
            if(index == -1) {
                balls.Add(new CountBall(type));
            }
            else {
                balls[index] += 1;
            }
        }

        public void RemoveBall(BallType type)
        {
            int index = balls.FindIndex(x => x.type == type);
            if (index == -1) {
                Debug.Log("Error: Try removing ball that's not exist in ball records");
            }
            else {
                balls[index] -= 1;
                if(balls[index].count == 0) {
                    balls.RemoveAt(index);
                }
            }
        }
    }

    [System.Serializable]
    public struct CountBall
    {
        public BallType type;
        public int count;

        public CountBall(BallType _type)
        {
            type = _type;
            count = 1;
        }

        public CountBall(BallType _type, int _count)
        {
            type = _type;
            count = _count;
        }

        public static CountBall operator + (CountBall obj, int val)
        {
            return new CountBall(obj.type, obj.count + val);
        }

        public static CountBall operator - (CountBall obj, int val)
        {
            return new CountBall(obj.type, obj.count - val);
        }
    }
}
