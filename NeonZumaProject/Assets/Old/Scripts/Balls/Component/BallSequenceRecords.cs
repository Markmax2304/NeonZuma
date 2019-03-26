using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class BallSequenceRecords
    {
        public List<BallSequence> sequences;
        float tailSpeed;
        float offsetBetweenBalls;
        float animSpeed;

        public BallSequenceRecords(float offset, float _animSpeed, float speed = 0f)
        {
            tailSpeed = speed;
            offsetBetweenBalls = offset;
            animSpeed = _animSpeed;
            sequences = new List<BallSequence>();
            sequences.Add(new BallSequence(tailSpeed, true));
        }

        #region Cut/Connect

        public void CutSequence(PathFollower ball, bool isForward)       //ball остаётся у задней цепи из двух полученых
        {
            int index = GetSequenceIndex(ball);
            BallSequence chain = sequences[index];
            int ballIndex = chain.GetBallIndex(ball.id);
            if(ballIndex == 0 || ballIndex == chain.balls.Count - 1) {
                return;
            }
            if(ballIndex == -1) {
                Debug.Log("Error: ball not found in any chains");
                return;
            }

            bool isTail = chain.isTail;
            List<PathFollower> forwardBalls;
            List<PathFollower> backBalls;
            if (isForward) {
                forwardBalls = chain.GetRange(0, ballIndex);
                backBalls = chain.GetRange(ballIndex, chain.balls.Count - ballIndex);
            }
            else {
                forwardBalls = chain.GetRange(0, ballIndex + 1);
                backBalls = chain.GetRange(ballIndex + 1, chain.balls.Count - ballIndex - 1);
            }

            sequences.RemoveAt(index);
            sequences.Insert(index, new BallSequence(0, forwardBalls));
            sequences.Insert(index + 1, new BallSequence(0, backBalls, isTail));
        }

        public void RemoveSequence(PathFollower ball)
        {
            BallSequence chain = GetSequence(ball);
            for(int i = 0; i < chain.balls.Count; i++) {
                chain.balls[i].DestroyBall();
            }
            sequences.Remove(chain);
        }

        public void ConnectSequences(PathFollower ball, PathFollower coll)
        {
            int ballSequenceIndex = GetSequenceIndex(ball);
            if (ballSequenceIndex == -1) {
                Debug.Log("Error: dont exist sequence that's keeping this ball = " + ball.name);
                return;
            }
            int collSequenceIndex = GetSequenceIndex(coll);
            if (collSequenceIndex == -1) {
                Debug.Log("Error: dont exist sequence that's keeping this ball = " + coll.name);
                return;
            }

            if (sequences[ballSequenceIndex].balls.Count > 1) {
                ball.ActivateEdgeTag(false);
            }
            if (sequences[collSequenceIndex].balls.Count > 1) {
                coll.ActivateEdgeTag(false);
            }

            // define which chain is behind
            int frontIndex, backIndex;
            if (collSequenceIndex - ballSequenceIndex == 1) {
                backIndex = collSequenceIndex;
                frontIndex = ballSequenceIndex;
            }
            else {
                frontIndex = collSequenceIndex;
                backIndex = ballSequenceIndex;
            }

            // align offset between balls
            BallSequence frontSequence = sequences[frontIndex];
            float distance = sequences[backIndex].balls[0].Distance;
            for (int i = frontSequence.balls.Count - 1; i >= 0; i--) {
                distance += offsetBetweenBalls;
                frontSequence.balls[i].MoveDistanceToPoint(distance, animSpeed);
            }

            // merge two chains to one
            bool isTail = sequences[backIndex].isTail;
            sequences[frontIndex].balls.AddRange(sequences[backIndex].balls);
            sequences[frontIndex].SetSpeed(0f);
            sequences[frontIndex].isTail = isTail;
            sequences.RemoveAt(backIndex);
        }
        #endregion

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

        public float GetCoveredDistance()
        {
            if (sequences.Count > 0) {
                if (sequences[sequences.Count - 1].balls.Count > 0) {
                    return sequences[sequences.Count - 1].balls[0].Distance;
                }
            }
            return 0;
        }

        #region Tail
        public BallSequence GetTail()
        {
            if (sequences.Count == 0) {
                sequences.Add(new BallSequence(tailSpeed, true));
            }
            return sequences[sequences.Count - 1];
        }

        public void AddToChainTail(PathFollower ball)
        {
            sequences[sequences.Count - 1].AddBall(ball);
        }

        public void SetTailSpeed(float speed)
        {
            GetTail().SetSpeed(speed);
            tailSpeed = speed;
        }

        public bool CheckTail(PathFollower ball)
        {
            BallSequence chain = GetSequence(ball);
            return chain != null ? chain.isTail : false;
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
            isTail = _isTail;
            balls = new List<PathFollower>();
            SetSpeed(_speed);
        }

        public BallSequence(float _speed, List<PathFollower> _balls, bool _isTail = false)
        {
            id = nextId++;
            isTail = _isTail;
            balls = _balls;
            balls[0].ActivateEdgeTag(true);
            balls[balls.Count - 1].ActivateEdgeTag(true);
            SetSpeed(_speed);
        }

        public List<PathFollower> GetRange(int index, int count)
        {
            return balls.GetRange(index, count);
        }

        public int GetBallIndex(int id)
        {
            return balls.FindIndex(x => x.id == id);
        }

        public float GetDistanceOfLastElement()
        {
            return balls.Count > 0 ? balls[balls.Count - 1].Distance : 0;
        }

        public void AddBall(PathFollower ball, int backIndex)
        {
            ActivateTagOnEdges(false);

            ball.SetAnimateSpeed(speed);
            balls.Insert(backIndex, ball);

            ActivateTagOnEdges(true);
        }

        public void AddBall(PathFollower ball)
        {
            ActivateTagOnEdges(false);

            ball.SetAnimateSpeed(speed);
            balls.Add(ball);

            ActivateTagOnEdges(true);
        }

        public void SetSpeed(float _speed)
        {
            speed = _speed;
            for (int i = 0; i < balls.Count; i++) {
                balls[i].SetAnimateSpeed(_speed);
            }
        }

        void ActivateTagOnEdges(bool active)
        {
            if (balls.Count == 0)
                return;

            balls[0].ActivateEdgeTag(active);
            balls[balls.Count - 1].ActivateEdgeTag(active);
        }
    }
}
