using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Core
{
    public class BallsController : MonoBehaviour
    {
        PoolObjectKeeper poolKeeper;
        PathCreator pathCreator;
        BallSpawner ballSpawner;
        [SerializeField] BallOpener ballOpener;
        [HideInInspector] public BallRandom randomizator;

        [SerializeField] GeneralLevelSettings settings;
        [SerializeField] float currentSpeed = 0f;
        float startLength;
        float preEndLength;

        [SerializeField] BallSequenceRecords sequencesRecords;
        [SerializeField] CountBallRecords countRecords;
        Queue<CommonHandler> actions;

        [Space]
        public bool isStart = true;
        public bool spawnEnable = true;
        public bool isMove = true;
        bool isBackTrack = false;
        bool isShifting = false;

        int countElementToSpawn = 0;
        BallInfo ballTypeToSpawn;

        void Start()
        {
            poolKeeper = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
            pathCreator = GetComponentInChildren<PathCreator>();
            ballSpawner = pathCreator.GetComponentInChildren<BallSpawner>();
            randomizator = GetComponent<BallRandom>();
            randomizator.Init(countRecords);

            countRecords = new CountBallRecords();
            sequencesRecords = new BallSequenceRecords(settings.offsetBetweenBalls, settings.animSpeed);
            actions = new Queue<CommonHandler>();

            startLength = pathCreator.path.length * settings.startLengthByPercent / 100f;
            preEndLength = pathCreator.path.length * settings.endLengthByPercent / 100f;

            ballSpawner.spawnBall += OnProcessAddingBalls;
            ballSpawner.removeBall += RemoveBallFromChain;
            ballOpener.subscribeBall += countRecords.AddBall;
            ballOpener.unsubscribeBall += countRecords.RemoveBall;

            LevelEventSystem.instance.Subscribe(LevelEventType.GameOver, OnGameOver);
            LevelEventSystem.instance.Subscribe(LevelEventType.Win, OnGameWin);
            LevelEventSystem.instance.Subscribe(LevelEventType.PauseEnter, PauseEnable);
            LevelEventSystem.instance.Subscribe(LevelEventType.PauseExit, PauseDisable);

            OnPreStarting();
        }

        public void OnDestroy()
        {
            ballSpawner.spawnBall -= OnProcessAddingBalls;
            ballSpawner.removeBall -= RemoveBallFromChain;
            ballOpener.subscribeBall -= countRecords.AddBall;
            ballOpener.unsubscribeBall -= countRecords.RemoveBall;

            LevelEventSystem.instance.Unsubscribe(LevelEventType.GameOver, OnGameOver);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.Win, OnGameWin);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.PauseEnter, PauseEnable);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.PauseExit, PauseDisable);
        }

        #region Events

        void OnGameOver()
        {
            spawnEnable = false;
            isBackTrack = true;
            currentSpeed = settings.defeatSpeed;
            // clear sequenceRecords
        }

        void OnGameWin()
        {

        }

        void PauseEnable()
        {
            isMove = false;     // не покрывает все возможности и не отключает анимацию
        }
        
        void PauseDisable()
        {
            isMove = true;
        }

        void OnProcessAddingBalls()
        {
            if (spawnEnable && isMove) {
                AddBallToEndOfChain();
            }
        }

        void OnPreStarting()
        {
            currentSpeed = settings.startSpeed;
            sequencesRecords.SetTailSpeed(currentSpeed);
            AddBallToEndOfChain();          // create first ball
        }

        void OnPostStarting()
        {
            isStart = false;
            currentSpeed = settings.middleSpeed;
            sequencesRecords.SetTailSpeed(currentSpeed);
            //turn on player controller and other systems
            LevelEventSystem.instance.InvokeEvent(LevelEventType.Start);
        }
        #endregion

        // event of collision projectile with chain of balls
        public void OnCollisionBalls(PathFollower forceBall, PathFollower collideBall)
        {
            InsertBallToChain(forceBall, collideBall);
            ShiftBallsOnInserting(forceBall);
            
            actions.Enqueue(delegate () {
                if (CheckTheSameBallAround(forceBall)) {
                    TryDestroyTheSameTypeBall(forceBall);
                }
                VerifyChainsOnEdge();
            });
        }

        // event collision of two chains (parameters is edge ball between these chains)
        public void OnConnectionBalls(PathFollower forceBall, PathFollower collideBall)
        {
            sequencesRecords.ConnectSequences(forceBall, collideBall);

            if (isShifting) {
                actions.Enqueue(delegate () {
                    //Debug.Log("Connecting");
                    PostConnectProcess(forceBall, collideBall);
                });
            }
            else {
                PostConnectProcess(forceBall, collideBall);
            }
            
        }

        void PostConnectProcess(PathFollower forceBall, PathFollower collideBall)
        {
            if (CheckTwoBallIsEqual(forceBall, collideBall)) {
                // little backtrack
                if (sequencesRecords.CheckTail(forceBall) || sequencesRecords.CheckTail(collideBall)) {
                    StartCoroutine(BackTrack());
                }
                TryDestroyTheSameTypeBall(forceBall);
            }
            VerifyChainsOnEdge();
        }

        IEnumerator BackTrack()
        {
            isBackTrack = true;
            float endSpeed = currentSpeed;
            currentSpeed = -settings.backTrackSpeed;
            sequencesRecords.SetTailSpeed(currentSpeed);
            float deltaSpeed = endSpeed - currentSpeed;
            while (currentSpeed < endSpeed) {
                currentSpeed += Time.deltaTime * deltaSpeed * 2;            // this defines execution time of speed interpolation to half second
                yield return null;
            }
            currentSpeed = endSpeed;
            sequencesRecords.SetTailSpeed(currentSpeed);
            isBackTrack = false;
        }

        #region Ball operation in Chain

        // add to spawn place
        void AddBallToEndOfChain()
        {
            if(countElementToSpawn == 0) {
                ballTypeToSpawn = randomizator.GetRandomBallSequence(out countElementToSpawn);
            }
            countElementToSpawn--;

            BallSequence tail = sequencesRecords.GetTail();
            float distance = tail.GetDistanceOfLastElement();
            distance -= settings.offsetBetweenBalls;

            PathFollower ball = poolKeeper.RealeseObject(ballSpawner.GetPosition()).GetComponent<PathFollower>();
            ball.Initialize(pathCreator, distance);
            ball.SetType(ballTypeToSpawn);
            ball.ConnectWithChain += OnConnectionBalls;

            tail.AddBall(ball);
        }

        void RemoveBallFromChain(PathFollower ball)         // can be work astable
        {
            BallSequence chain = sequencesRecords.GetSequence(ball);
            int ballIndex = chain.GetBallIndex(ball.id);
            if(ballIndex + 1 < chain.balls.Count) {
                PathFollower nextBall = chain.balls[ballIndex + 1];
                nextBall.DestroyBall();
                chain.balls.RemoveAt(ballIndex + 1);
            }
        }

        // insert relatively collided ball
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

            // algorithm of choosing inserting side relatively collided ball
            Vector2 ballPosition = ball.GetPosition();
            float offsetBetweenBalls = settings.offsetBetweenBalls;
            float collDistance = Vector2.Distance(ballPosition, coll.GetPosition());
            float frontCollDistance = Vector2.Distance(ballPosition, coll.GetBackPosition(-offsetBetweenBalls));
            float backCollDistance = Vector2.Distance(ballPosition, coll.GetBackPosition(offsetBetweenBalls));

            // cosinus theorem
            float frontCosinus = (frontCollDistance * frontCollDistance - collDistance * collDistance - offsetBetweenBalls * offsetBetweenBalls)
                    / (-2 * collDistance * offsetBetweenBalls);
            float backCosinus = (backCollDistance * backCollDistance - collDistance * collDistance - offsetBetweenBalls * offsetBetweenBalls)
                    / (-2 * collDistance * offsetBetweenBalls);

            int collIndex = collidingSequence.GetBallIndex(coll.id);
            int frontBallIndex = frontCosinus > backCosinus ? collIndex - 1 : collIndex;
            int backBallIndex = frontBallIndex + 1;

            // initialize new ball
            float distance = frontBallIndex == -1 ? collidingSequence.balls[0].Distance + offsetBetweenBalls : collidingSequence.balls[frontBallIndex].Distance;
            ball.Initialize(pathCreator, distance);
            ball.ConnectWithChain += OnConnectionBalls;
            collidingSequence.AddBall(ball, backBallIndex);

            countRecords.AddBall(ball.type);
        }

        void ShiftBallsOnInserting(PathFollower newBall)
        {
            BallSequence sequence = sequencesRecords.GetSequence(newBall);
            int index = sequence.GetBallIndex(newBall.id);

            if (sequence.isTail) {
                isMove = false;
            }
            isShifting = true;
            sequence.SetSpeed(0f);

            for (int i = 0; i < index; i++) {
                sequence.balls[i].MoveDistance(settings.offsetBetweenBalls, settings.timeForInsertingAnimation);
            }
            newBall.MoveDistance(0, settings.timeForInsertingAnimation, delegate () {
                isShifting = false;
                isMove = true;

                while(actions.Count > 0) {
                    actions.Dequeue()();
                }
            });
        }

        bool CheckTheSameBallAround(PathFollower ball)
        {
            BallSequence sequence = sequencesRecords.GetSequence(ball);
            if (sequence == null) {
                Debug.Log("Error: Ball(" + ball.name + ") isn't belonged in any balls sequence");
                isMove = false;
                return false;
            }
            int index = sequence.GetBallIndex(ball.id);

            if (index + 1 < sequence.balls.Count && sequence.balls[index + 1].type == ball.type) {
                return true;
            }
            if (index - 1 >= 0 && sequence.balls[index - 1].type == ball.type) {
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
            return first.type == second.type;
        }

        void TryDestroyTheSameTypeBall(PathFollower ball)
        {
            BallSequence sequence = sequencesRecords.GetSequence(ball);
            int index = sequence.GetBallIndex(ball.id);
            BallType typeBall = ball.type;

            // calculate quantity of balls with the same color on the front and behind
            int forwardIndex = index;
            for(; forwardIndex >= 0; forwardIndex--) {
                if (sequence.balls[forwardIndex].type != typeBall) {
                    break;
                }
            }
            int backIndex = index;
            for (; backIndex < sequence.balls.Count; backIndex++) {
                if (sequence.balls[backIndex].type != typeBall) {
                    break;
                }
            }
            forwardIndex++; backIndex--;

            int length = backIndex - forwardIndex + 1;
            if (length > 2) {
                sequencesRecords.CutSequence(sequence.balls[forwardIndex], true);
                sequencesRecords.CutSequence(sequence.balls[backIndex], false);
                sequencesRecords.RemoveSequence(ball);

                countRecords.RemoveBall(typeBall, length);
            }
        }

        void VerifyChainsOnEdge()
        {
            //Debug.Log("Test: verify attraction");
            ClearAttractionChainToChain();
            for(int i = 0; i < sequencesRecords.sequences.Count - 1; i++) {
                BallSequence fronSequence = sequencesRecords.sequences[i];
                BallSequence backSequence = sequencesRecords.sequences[i + 1];

                if(fronSequence.balls[fronSequence.balls.Count - 1].type == backSequence.balls[0].type) {
                    SetAttractionChainToChain(fronSequence, backSequence);
                }
            }
            sequencesRecords.sequences[sequencesRecords.sequences.Count - 1].SetSpeed(currentSpeed);
        }

        void SetAttractionChainToChain(BallSequence frontChain, BallSequence backChain)
        {
            if (!backChain.isTail) {
                backChain.SetSpeed(CalculateAttractiveSpeed(backChain));
            }
            frontChain.SetSpeed(-CalculateAttractiveSpeed(frontChain));
        }

        void ClearAttractionChainToChain()
        {
            for (int i = 0; i < sequencesRecords.sequences.Count; i++) {
                sequencesRecords.sequences[i].SetSpeed(0f);
            }
        }

        float CalculateAttractiveSpeed(BallSequence chain)
        {
            int count = chain.balls.Count;
            float rate = 1;
            int i = 2;
            while (Mathf.Pow(i, 2) <= count) {
                i++;
                rate *= settings.decreaseRate;
            }
            //Debug.Log(maxAttractiveSpeed * rate);
            return settings.maxAttractiveSpeed * rate;
        }
        #endregion

        #region Movement

        void FixedUpdate()
        {
            if (isMove) {
                MoveAllBalls(Time.fixedDeltaTime);
            }

            if (isStart) {
                if (sequencesRecords.GetCoveredDistance() >= startLength) {
                    OnPostStarting();
                }
            }
            else if(!isBackTrack) {
                float coveredDistance = sequencesRecords.GetCoveredDistance();
                if (currentSpeed != settings.middleSpeed && coveredDistance < preEndLength) {
                    currentSpeed = settings.middleSpeed;
                    sequencesRecords.SetTailSpeed(currentSpeed);
                }
                if(currentSpeed != settings.endSpeed && coveredDistance >= preEndLength) {
                    currentSpeed = settings.endSpeed;
                    sequencesRecords.SetTailSpeed(currentSpeed);
                }
            }
        }

        void MoveAllBalls(float delta)
        {
            float rateDelta;
            for (int i = 0; i < sequencesRecords.sequences.Count - 1; i++) {
                BallSequence sequence = sequencesRecords.sequences[i];
                rateDelta = delta * sequence.speed;
                for (int j = 0; j < sequence.balls.Count; j++) {
                    sequence.balls[j].MoveUpdate(rateDelta);
                }
            }

            // tail moves apart
            BallSequence tailSecuence = sequencesRecords.GetTail();
            rateDelta = delta * currentSpeed;
            for (int j = 0; j < tailSecuence.balls.Count; j++) {
                tailSecuence.balls[j].MoveUpdate(rateDelta);
            }
        }
        #endregion

        #region DebugGizmoz

        Color[] colors = { Color.blue, Color.green, Color.red, Color.yellow, Color.white, Color.magenta, Color.cyan };

        void OnDrawGizmos()
        {
            float count = sequencesRecords.sequences.Count;
            for (int i = 0; i < 7 && i < count; i++) {
                Color color = colors[i];
                for(int j = 0; j < sequencesRecords.sequences[i].balls.Count; j++) {
                    Vector2 pos = sequencesRecords.sequences[i].balls[j].GetPosition();
                    Vector2 dir = Vector2.zero - pos;
                    Debug.DrawRay(pos, dir.normalized, color);
                }
            }
        }
        #endregion
    }
}
