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

        [SerializeField] BallSequenceRecords sequencesRecords;
        /*[SerializeField]*/ CountBallRecords countRecords;
        string tagBall = "Chain";

        [Space]
        public bool spawnEnable = true;
        public bool isMove = true;

        int countElementToSpawn = 0;
        BallInfo ballTypeToSpawn;

        void Start()
        {
            countRecords = new CountBallRecords();
            sequencesRecords = new BallSequenceRecords(speed);
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

            BallSequence tail = sequencesRecords.GetTail();
            int ballsCount = tail.balls.Count;
            float distance = ballsCount > 0 ? tail.balls[ballsCount - 1].Distance : 0;
            distance -= offsetBeetweenBalls;

            PathFollower ball = poolKeeper.RealeseObject(spawnPosition).GetComponent<PathFollower>();
            ball.GetComponent<Ball>().Initialize(ballTypeToSpawn);
            ball.Initialize(pathCreator, distance);
            ball.tag = tagBall;
            sequencesRecords.AddToChainTail(ball);
            countRecords.AddBall(ballTypeToSpawn.type);
        }

        // вставляет шар в цепь относительно шара с которым столкнулся
        public void InsertBallToChain(PathFollower ball, PathFollower coll)
        {
            if (ball == null || coll == null) {
                Debug.Log("Error: Collision happened but some object is null");
                return;
            }

            BallSequence collidingSequence = sequencesRecords.GetSequence(coll);
            if(collidingSequence == null) {
                Debug.Log("Error: Not found sequences that store this ball");
                return;
            }
            int collIndex = collidingSequence.balls.FindIndex(x => x.id == coll.id);
            int frontBallIndex = 0, backBallIndex = 0;

            // алгоритм не оптимальный, но рабочий
            // столкновение с первым шаром рассматриваем как особый случай
            if(collIndex == 0) {
                // теорема косинусов
                float collDistance = Vector2.Distance(ball.transform.position, collidingSequence.balls[collIndex].transform.position);
                float backCollDistance = Vector2.Distance(ball.transform.position, collidingSequence.balls[collIndex + 1].transform.position);
                float cosinus = (backCollDistance * backCollDistance - collDistance * collDistance - offsetBeetweenBalls * offsetBeetweenBalls)
                    / (-2 * collDistance * offsetBeetweenBalls);

                if (cosinus >= 0) {
                    frontBallIndex = 0;
                }
                else {
                    frontBallIndex = -1;
                }
                backBallIndex = frontBallIndex + 1;
            }
            else if(collIndex > 0) {

                if (collIndex == -1) {
                    Debug.Log("Error: ball is collided with something unexpected and that exactly isn't chain of balls\n" +
                        "Distance = " + collIndex);
                    isMove = false;
                    return;
                }

                // теорема косинусов
                float collDistance = Vector2.Distance(ball.transform.position, collidingSequence.balls[collIndex].transform.position);
                float frontCollDistance = Vector2.Distance(ball.transform.position, collidingSequence.balls[collIndex - 1].transform.position);
                float cosinus = (frontCollDistance * frontCollDistance - collDistance * collDistance - offsetBeetweenBalls * offsetBeetweenBalls)
                    / (-2 * collDistance * offsetBeetweenBalls);        

                if (cosinus >= 0) {
                    frontBallIndex = collIndex - 1;
                }
                else {
                    frontBallIndex = collIndex;
                }
                backBallIndex = frontBallIndex + 1;
            }

            float distance = frontBallIndex == -1 ? collidingSequence.balls[0].Distance + offsetBeetweenBalls : collidingSequence.balls[frontBallIndex].Distance;
            ball.Initialize(pathCreator, distance);
            ball.tag = tagBall;
            ShiftBallsOnInserting(ball, collidingSequence, frontBallIndex);
            collidingSequence.AddBall(ball, backBallIndex);
            countRecords.AddBall(ball.GetComponent<Ball>().Type);
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
            int index = Random.Range(0, countRecords.balls.Count);
            return countRecords.balls[index].type;
        }

        bool CheckTheSameBallAround(BallSequence sequence, PathFollower ball)
        {
            int index = sequence.balls.FindIndex(x => x.id == ball.id);
            if(index == -1) {
                Debug.Log("Error: Ball(" + ball.id + ") dont exist in balls sequence(" + sequence.id + ")");
                isMove = false;
                return false;
            }

            if (index + 1 < sequence.balls.Count && sequence.balls[index + 1].GetTypeBall() == ball.GetTypeBall()) {
                return true;
            }
            if (index - 1 >= 0 && sequence.balls[index - 1].GetTypeBall() == ball.GetTypeBall()) {
                return true;
            }
            return false;
        }

        void TryDestroyTheSameTypeBall(BallSequence sequence, PathFollower ball)
        {
            int index = sequence.balls.FindIndex(x => x.id == ball.id);
            BallType typeBall = ball.GetTypeBall();

            int forwardIndex = index;
            for(; forwardIndex >= 0; forwardIndex--) {
                if (sequence.balls[forwardIndex].GetTypeBall() != typeBall) {
                    break;
                }
            }
            int backIndex = index;
            for (; backIndex < sequence.balls.Count; backIndex++) {
                if (sequence.balls[backIndex].GetTypeBall() != typeBall) {
                    break;
                }
            }
            forwardIndex++; backIndex--;

            int length = backIndex - forwardIndex + 1;
            if (length > 2) {
                for(int i = forwardIndex; i <= backIndex; i++) {
                    sequence.balls[i].GetComponent<PoolingObject>().ReturnToPool();
                    // destroy effect
                }
                sequencesRecords.CutSequenceByRemoving(sequence.id, forwardIndex, length);
                countRecords.RemoveBall(typeBall, length);
                
            }
        }
        #endregion

        #region Movement

        void FixedUpdate()
        {
            if(isMove)
                MoveAllBalls(Time.fixedDeltaTime);
        }

        void MoveAllBalls(float delta)
        {
            for (int i = 0; i < sequencesRecords.sequences.Count; i++) {
                BallSequence sequence = sequencesRecords.sequences[i];
                float rateDelta = delta * sequence.speed;
                for (int j = 0; j < sequence.balls.Count; j++) {
                    sequence.balls[j].MoveUpdate(rateDelta);
                }
            }
        }

        void ShiftBallsOnInserting(PathFollower newBall, BallSequence sequence, int index)
        {
            isMove = false;

            for (int i = 0; i <= index; i++) {
                sequence.balls[i].MoveDistance(offsetBeetweenBalls, timeForInsertingAnimation);
            }
            newBall.MoveDistance(0, timeForInsertingAnimation, delegate () {
                isMove = true;
                if (CheckTheSameBallAround(sequence, newBall)) {
                    TryDestroyTheSameTypeBall(sequence, newBall);
                }
            });
        }
        #endregion

        #region DebugGizmoz

        void OnDrawGizmos()
        {
            
        }
        #endregion
    }

    #region Counting Balls

    [System.Serializable]
    public class CountBallRecords
    {
        public List<CountBall> balls;

        public CountBallRecords()
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

        public void RemoveBall(BallType type, int count)
        {
            int index = balls.FindIndex(x => x.type == type);
            if (index == -1) {
                Debug.Log("Error: Try removing ball that's not exist in ball records");
            }
            else {
                balls[index] -= count;
                if (balls[index].count <= 0) {
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
    #endregion

    #region Sequences of balls

    [System.Serializable]
    public class BallSequenceRecords
    {
        public List<BallSequence> sequences;

        public BallSequenceRecords(float _speed)
        {
            sequences = new List<BallSequence>();
            sequences.Add(new BallSequence(_speed));
        }

        public void CutSequenceByRemoving(int idSequence, int startRemove, int lengthRemove)
        {
            int index = sequences.FindIndex(x => x.id == idSequence);
            int restStart = startRemove + lengthRemove;
            int restLength = sequences[index].balls.Count - restStart;

            if (startRemove > 0 && restLength > 0) {
                List<PathFollower> forwardBalls = sequences[index].balls.GetRange(0, startRemove);
                List<PathFollower> backBalls = sequences[index].balls.GetRange(restStart, restLength);

                float speed = sequences[index].speed;
                sequences.RemoveAt(index);
                sequences.Insert(index, new BallSequence(speed, forwardBalls));
                sequences.Insert(index + 1, new BallSequence(speed, backBalls));    // change it
            }
            else {
                sequences[index].balls.RemoveRange(startRemove, lengthRemove);
                if (sequences[index].balls.Count == 0) {
                    sequences.RemoveAt(index);
                }
            }
        }

        public void ConnectSequences()
        {

        }

        public BallSequence GetSequence(PathFollower ball)      //will test
        {
            return sequences.Find(x => x.balls.Find(y => y.id == ball.id) != null);
        }

        public void AddToSequence(PathFollower ball, int idSequence)
        {
            int index = sequences.FindIndex(x => x.id == idSequence);
            sequences[index].balls.Add(ball);
        }

        // maybe change to PathFollower?
        public void SetSequenceSpeed(float speed, int idSequence)
        {
            int index = sequences.FindIndex(x => x.id == idSequence);
            sequences[index].SetSpeed(speed);
        }

        #region Tail
        public BallSequence GetTail()
        {
            return sequences[sequences.Count - 1];
        }

        public void AddToChainTail(PathFollower ball)
        {
            sequences[sequences.Count - 1].balls.Add(ball);
        }
        #endregion
    }

    [System.Serializable]
    public class BallSequence
    {
        static int nextId = 0;
        public int id;
        public float speed;
        public List<PathFollower> balls;

        public BallSequence(float _speed)
        {
            id = nextId++;
            speed = _speed;
            balls = new List<PathFollower>();
        }

        public BallSequence(float _speed, List<PathFollower> _balls)
        {
            id = nextId++;
            speed = _speed;
            balls = _balls;
            balls[0].ActivateEdgeTag(true);
            balls[balls.Count - 1].ActivateEdgeTag(true);
        }

        public void AddBall(PathFollower ball, int backIndex)
        {
            if (backIndex == 0) {
                balls[0].ActivateEdgeTag(false);
                ball.ActivateEdgeTag(true);
                balls.Insert(0, ball);
            }
            else if (backIndex == balls.Count) {
                balls[backIndex - 1].ActivateEdgeTag(false);
                ball.ActivateEdgeTag(true);
                balls.Add(ball);
            }
            else {
                balls.Insert(backIndex, ball);
            }
        }

        public void SetSpeed(float _speed)
        {
            speed = _speed;
        }
    }
    #endregion
}
