using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using DG.Tweening;

namespace Core
{
    public class PathFollower : MonoBehaviour
    {
        public EndOfPathInstruction endOfPathInstruction;
        public float distanceTravelled;
        public int id;
        static int nextId = 0;

        PathCreator pathCreator;
        Transform _trasform;
        Ball ball;

        public void Initialize(PathCreator path, float distance = 0)
        {
            id = nextId++;
            _trasform = transform;
            pathCreator = path;
            ball = GetComponent<Ball>();
            distanceTravelled = distance;
        }

        public BallType GetTypeBall()
        {
            return ball.Type;
        }

        public float Distance {
            get { return distanceTravelled; }       
            set { distanceTravelled = value; }
        }

        #region Movement

        public void MoveUpdate(float delta)
        {
            if (pathCreator != null) {
                distanceTravelled += delta;
                _trasform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        public void MoveDistance(float distance, float time, TweenCallback action)
        {
            distanceTravelled += distance;
            Vector2 position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            _trasform.DOMove(position, time).OnComplete(action);
        }

        public void MoveDistance(float distance, float time)
        {
            distanceTravelled += distance;
            Vector2 position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            _trasform.DOMove(position, time);
        }
        #endregion
    }
}
