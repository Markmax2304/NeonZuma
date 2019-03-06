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
        public float speed = .5f;
        public float attractiveSpeed = 2f;
        public float animSpeed = 3f;                // for animation of connection and lining
        public float offsetBeetweenBalls = .36f;
        public float timeForInsertingAnimation = .15f;          // time for animation of inserting

        [SerializeField] BallSequenceRecords sequencesRecords;
        [SerializeField] CountBallRecords countRecords;
        Queue<BallHandler> actions;
        string tagBall = "Chain";

        [Space]
        public bool spawnEnable = true;
        public bool isMove = true;
        bool isShifting = false;

        int countElementToSpawn = 0;
        BallInfo ballTypeToSpawn;

        void Start()
        {
            countRecords = new CountBallRecords();
            sequencesRecords = new BallSequenceRecords(speed);
            poolKeeper = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
            randomizator = GetComponent<BallRandom>();
            actions = new Queue<BallHandler>();

            StartCoroutine(ProcessAddingBalls());
        }

        public BallType GetNextBallType()
        {
            int index = Random.Range(0, countRecords.balls.Count);
            return countRecords.balls[index].type;
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

        #region Ball operation in Chain

        public void OnCollisionBalls(PathFollower forceBall, PathFollower collideBall)
        {
            InsertBallToChain(forceBall, collideBall);
            ShiftBallsOnInserting(forceBall);

            actions.Enqueue(delegate () {
                if (CheckTheSameBallAround(forceBall)) {
                    TryDestroyTheSameTypeBall(forceBall);
                }
            });
        }

        public void OnConnectionBalls(PathFollower forceBall, PathFollower collideBall)
        {
            ConnectSequences(forceBall, collideBall);

            if (isShifting) {
                actions.Enqueue(delegate () {
                    //Debug.Log("Connecting");
                    if(CheckTwoBallIsEqual(forceBall, collideBall)) {
                        TryDestroyTheSameTypeBall(forceBall);       // will test
                    }
                });
            }
            else {
                if (CheckTwoBallIsEqual(forceBall, collideBall)) {
                    TryDestroyTheSameTypeBall(forceBall);       // will test
                }
            }
            
        }

        // добавляет шар в конец всех цепей, в месте спауна
        void AddBallToEndOfChain()
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
            ball.ConnectWithChain += OnConnectionBalls;

            sequencesRecords.AddToChainTail(ball);
            countRecords.AddBall(ballTypeToSpawn.type);
        }

        // вставляет шар в цепь относительно шара с которым столкнулся
        void InsertBallToChain(PathFollower ball, PathFollower coll)
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

            int collIndex = collidingSequence.GetBallIndex(coll.id);

            // алгоритм выбора с какой стороны от шара столкновения присоединиться в цепь шаров
            Vector2 ballPosition = ball.transform.position;
            float collDistance = Vector2.Distance(ballPosition, collidingSequence.balls[collIndex].transform.position);
            float frontCollDistance = Vector2.Distance(ballPosition, collidingSequence.balls[collIndex].GetBackPosition(-offsetBeetweenBalls));
            float backCollDistance = Vector2.Distance(ballPosition, collidingSequence.balls[collIndex].GetBackPosition(offsetBeetweenBalls));

            // теорема синусов
            float frontCosinus = (frontCollDistance * frontCollDistance - collDistance * collDistance - offsetBeetweenBalls * offsetBeetweenBalls)
                    / (-2 * collDistance * offsetBeetweenBalls);
            float backCosinus = (backCollDistance * backCollDistance - collDistance * collDistance - offsetBeetweenBalls * offsetBeetweenBalls)
                    / (-2 * collDistance * offsetBeetweenBalls);

            // индексы элементов между которыми вставляем шар
            int frontBallIndex = frontCosinus > backCosinus ? collIndex - 1 : collIndex;
            int backBallIndex = frontBallIndex + 1;

            // инициализация нового шара
            float distance = frontBallIndex == -1 ? collidingSequence.balls[0].Distance + offsetBeetweenBalls : collidingSequence.balls[frontBallIndex].Distance;
            ball.Initialize(pathCreator, distance);
            ball.ConnectWithChain += OnConnectionBalls;
            collidingSequence.AddBall(ball, backBallIndex);

            countRecords.AddBall(ball.GetTypeBall());
        }

        void ShiftBallsOnInserting(PathFollower newBall)
        {
            BallSequence sequence = sequencesRecords.GetSequence(newBall);
            int index = sequence.GetBallIndex(newBall.id);

            isShifting = true;
            float speed = sequence.speed;

            for (int i = 0; i < index; i++) {
                sequence.balls[i].MoveDistance(offsetBeetweenBalls, timeForInsertingAnimation);
            }
            newBall.MoveDistance(0, timeForInsertingAnimation, delegate () {
                isShifting = false;
                sequencesRecords.GetSequence(newBall).SetSpeed(speed);          // такое присвоение скорости слегка опасно, могут быть ошибки

                while(actions.Count > 0) {
                    actions.Dequeue()();
                }
            });
        }

        bool CheckTheSameBallAround(PathFollower ball)
        {
            BallSequence sequence = sequencesRecords.GetSequence(ball);
            if (sequence == null) {
                Debug.Log("Error: Ball(" + ball.id + ") isn't belonged in any balls sequence");
                isMove = false;
                return false;
            }
            int index = sequence.GetBallIndex(ball.id);

            if (index + 1 < sequence.balls.Count && sequence.balls[index + 1].GetTypeBall() == ball.GetTypeBall()) {
                return true;
            }
            if (index - 1 >= 0 && sequence.balls[index - 1].GetTypeBall() == ball.GetTypeBall()) {
                return true;
            }
            return false;
        }

        bool CheckTwoBallIsEqual(PathFollower first, PathFollower second)
        {
            if(sequencesRecords.GetSequence(first) == null || sequencesRecords.GetSequence(second) == null) {
                //Debug.Log("Connecting balls was destroying");
                return false;
            }
            return first.GetTypeBall() == second.GetTypeBall();
        }

        void TryDestroyTheSameTypeBall(PathFollower ball)
        {
            BallSequence sequence = sequencesRecords.GetSequence(ball);
            int index = sequence.GetBallIndex(ball.id);
            BallType typeBall = ball.GetTypeBall();

            // подсчитываем количество шаров такого же цвета справа и слева от проверяемого шара(ball)
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
                CutSequenceByRemoving(sequence.id, forwardIndex, length);
                countRecords.RemoveBall(typeBall, length);

                // проверка на соответствие шаров по краям разрыва
                // в случае соответствия - взаимное притяжение
                // скорость притяжения зависит от количества шаров в последовательности: чем меньше шаров, тем больше скорость
                VerifyChainsOnEdge();
            }
        }

        void VerifyChainsOnEdge()
        {
            for(int i = 0; i < sequencesRecords.sequences.Count - 1; i++) {
                BallSequence fronSequence = sequencesRecords.sequences[i];
                BallSequence backSequence = sequencesRecords.sequences[i + 1];

                if(fronSequence.balls[fronSequence.balls.Count - 1].GetTypeBall() == backSequence.balls[0].GetTypeBall()) {
                    Debug.Log("Attraction");
                    SetAttractionChainToChain(fronSequence, backSequence);
                }
            }
        }

        void SetAttractionChainToChain(BallSequence frontChain, BallSequence backChain)
        {
            if (!backChain.isTail) {
                backChain.SetSpeed(attractiveSpeed);
            }
            frontChain.SetSpeed(-attractiveSpeed);
        }
        #endregion

        #region Cut/Connect

        public void CutSequenceByRemoving(int idSequence, int startRemove, int lengthRemove)
        {
            int index = sequencesRecords.GetSequenceIndex(idSequence);
            BallSequence sequence = sequencesRecords.sequences[index];
            if (index == -1) {
                Debug.Log("Error: cant cut sequence(" + idSequence + ") in removing from = " + startRemove + "  length = " + lengthRemove);
                return;
            }
            int restStart = startRemove + lengthRemove;
            int restLength = sequence.balls.Count - restStart;
            bool isTail = sequence.isTail;

            if (startRemove > 0 && restLength > 0) {
                List<PathFollower> forwardBalls = sequence.balls.GetRange(0, startRemove);
                List<PathFollower> backBalls = sequence.balls.GetRange(restStart, restLength);

                float speed = sequence.speed;       // выбирать исходя из того, куда двигалась последовательность(исключая основание последовательностей)
                sequencesRecords.sequences.RemoveAt(index);
                sequencesRecords.sequences.Insert(index, new BallSequence(0, forwardBalls));             // шары, что впереди от разрыва по направлению движения
                sequencesRecords.sequences.Insert(index + 1, new BallSequence(speed, backBalls, isTail));        // шары, что сзади от разрыва
            }
            else {
                sequence.balls.RemoveRange(startRemove, lengthRemove);
                if (sequence.balls.Count == 0) {
                    sequencesRecords.sequences.RemoveAt(index);
                }
                else {
                    if (startRemove == 0) {
                        sequence.balls[0].ActivateEdgeTag(true);
                    }
                    else if (restLength == 0) {
                        int last = sequence.balls.Count - 1;
                        sequence.balls[last].ActivateEdgeTag(true);
                    }
                    else {
                        Debug.Log("Error: On deleting balls from edge of chain");
                    }
                }
            }
        }

        public void ConnectSequences(PathFollower ball, PathFollower coll)
        {
            int ballSequenceIndex = sequencesRecords.GetSequenceIndex(ball);
            if (ballSequenceIndex == -1) {
                Debug.Log("Error: dont exist sequence that's keeping this ball = " + ball.name);
                return;
            }
            int collSequenceIndex = sequencesRecords.GetSequenceIndex(coll);
            if (collSequenceIndex == -1) {
                Debug.Log("Error: dont exist sequence that's keeping this ball = " + coll.name);
                return;
            }

            // убираем теги Edge если наши шары не единственные в добвляемой цепи (тогда они снова будут крайними и менять тег нет смысла)
            if (sequencesRecords.sequences[ballSequenceIndex].balls.Count > 1) {
                ball.ActivateEdgeTag(false);
            }
            if (sequencesRecords.sequences[collSequenceIndex].balls.Count > 1) {
                coll.ActivateEdgeTag(false);
            }

            // определение которая последовательность является задней
            int frontIndex, backIndex;
            if (collSequenceIndex - ballSequenceIndex == 1) {
                backIndex = collSequenceIndex;
                frontIndex = ballSequenceIndex;
            }
            else {
                frontIndex = collSequenceIndex;
                backIndex = ballSequenceIndex;
            }

            // сравняли расстояние между шарами
            BallSequence frontSequence = sequencesRecords.sequences[frontIndex];
            float distance = sequencesRecords.sequences[backIndex].balls[0].Distance;
            for (int i = frontSequence.balls.Count - 1; i >= 0; i--) {
                distance += offsetBeetweenBalls;                        //change to static variable
                frontSequence.balls[i].MoveDistanceToPoint(distance, animSpeed);
            }

            // непосредственно добавляем шары одной цепи в шары другой
            bool isTail = sequencesRecords.sequences[backIndex].isTail;
            float speed = isTail ? sequencesRecords.sequences[backIndex].speed : 0f;
            sequencesRecords.sequences[frontIndex].balls.AddRange(sequencesRecords.sequences[backIndex].balls);
            sequencesRecords.sequences[frontIndex].SetSpeed(speed);
            sequencesRecords.sequences[frontIndex].isTail = isTail;
            sequencesRecords.sequences.RemoveAt(backIndex);
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
            sequences.Add(new BallSequence(_speed, true));
        }

        public BallSequence GetSequence(PathFollower ball)
        {
            return sequences.Find(x => x.balls.Find(y => y.id == ball.id) != null);
        }

        public int GetSequenceIndex(int idSequence)
        {
            return sequences.FindIndex(x => x.id == idSequence);
        }

        public int GetSequenceIndex(PathFollower ball)
        {
            return sequences.FindIndex(x => x.balls.Find(y => y.id == ball.id) != null);
        }

        //excess?
        public void AddToSequence(PathFollower ball, int idSequence)
        {
            int index = sequences.FindIndex(x => x.id == idSequence);
            sequences[index].balls.Add(ball);
        }

        // maybe change to PathFollower?            excess?
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
            sequences[sequences.Count - 1].AddBall(ball);
        }
        #endregion
    }

    [System.Serializable]
    public class BallSequence
    {
        static int nextId = 0;
        public int id;
        public float speed;
        public bool isTail;
        public List<PathFollower> balls;

        public BallSequence(float _speed, bool _isTail = false)
        {
            id = nextId++;
            speed = _speed;
            isTail = _isTail;
            balls = new List<PathFollower>();
        }

        public BallSequence(float _speed, List<PathFollower> _balls, bool _isTail = false)
        {
            id = nextId++;
            speed = _speed;
            isTail = _isTail;
            balls = _balls;
            balls[0].ActivateEdgeTag(true);
            balls[balls.Count - 1].ActivateEdgeTag(true);
        }

        public int GetBallIndex(int id)
        {
            return balls.FindIndex(x => x.id == id);
        }
        
        public void AddBall(PathFollower ball, int backIndex)
        {
            if (backIndex == 0) {
                balls[0].ActivateEdgeTag(false);
                ball.ActivateEdgeTag(true);
                balls.Insert(0, ball);
            }
            else if (backIndex == balls.Count) {
                AddBall(ball);
            }
            else {
                balls.Insert(backIndex, ball);
            }
        }

        public void AddBall(PathFollower ball)
        {
            if (balls.Count > 0) {
                balls[balls.Count - 1].ActivateEdgeTag(false);
            }
            ball.ActivateEdgeTag(true);
            balls.Add(ball);
        }

        /*public void SubsribeConnectingMethodToFirstBall(BallCollisionHandler handler)
        {
            //Debug.Log("Subsribe " + balls[0].name);
            balls[0].ConnectWithChain += handler;
        }*/

        public void SetSpeed(float _speed)
        {
            speed = _speed;
        }
    }
    #endregion
}
