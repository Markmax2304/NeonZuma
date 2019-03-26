using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class CountBallRecords
    {
        public List<CountBall> balls;

        public CountBallRecords()
        {
            balls = new List<CountBall>();
        }

        public void AddBall(PathFollower ball)
        {
            AddBall(ball.type);
        }

        public void AddBall(BallType type)
        {
            int index = balls.FindIndex(x => x.type == type);
            if (index == -1) {
                balls.Add(new CountBall(type));
            }
            else {
                balls[index] += 1;
            }
        }

        public void RemoveBall(PathFollower ball)
        {
            RemoveBall(ball.type);
        }

        public void RemoveBall(BallType type)
        {
            int index = balls.FindIndex(x => x.type == type);
            if (index == -1) {
                Debug.Log("Error: Try removing ball that's not exist in ball records");
            }
            else {
                balls[index] -= 1;
                if (balls[index].count == 0) {
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

        public static CountBall operator +(CountBall obj, int val)
        {
            return new CountBall(obj.type, obj.count + val);
        }

        public static CountBall operator -(CountBall obj, int val)
        {
            return new CountBall(obj.type, obj.count - val);
        }
    }
}
