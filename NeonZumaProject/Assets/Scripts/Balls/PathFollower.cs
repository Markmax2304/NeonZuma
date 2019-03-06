using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using DG.Tweening;

namespace Core
{
    public class PathFollower : MonoBehaviour
    {
        public event BallCollisionHandler ConnectWithChain;

        public EndOfPathInstruction endOfPathInstruction;
        public float distanceTravelled;
        public int id;
        static int nextId = 0;

        PathCreator pathCreator;
        Transform _trasform;
        Ball ball;

        void Awake()
        {
            _trasform = transform;
        }

        public void Initialize(PathCreator path, float distance = 0)
        {
            id = nextId++;
            tag = "Chain";
            pathCreator = path;
            ball = GetComponent<Ball>();
            distanceTravelled = distance;
        }

        #region Property

        public BallType GetTypeBall()
        {
            return ball.Type;
        }

        public float Distance {
            get { return distanceTravelled; }       
            set { distanceTravelled = value; }
        }

        public Vector2 GetBackPosition(float offset)
        {
            return pathCreator.path.GetPointAtDistance(distanceTravelled - offset);
        }

        public void ActivateEdgeTag(bool active)
        {
            if (active)
                _trasform.tag = "Edge";
            else
                _trasform.tag = "Chain";
        }
        #endregion

        #region Movement

        public void MoveUpdate(float delta)
        {
            if (pathCreator != null) {
                distanceTravelled += delta;
                _trasform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        public void MoveDistanceToPoint(float distancePoint, float speed, TweenCallback action)
        {
            float time = Mathf.Abs(distancePoint - distanceTravelled) / speed;
            //Debug.Log("connect anim time = " + time);
            distanceTravelled = distancePoint;
            Vector2 position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            _trasform.DOMove(position, time).OnComplete(action);
        }

        public void MoveDistanceToPoint(float distancePoint, float speed)
        {
            MoveDistanceToPoint(distancePoint, speed, delegate () { });
        }

        public void MoveDistance(float distance, float time, TweenCallback action)
        {
            distanceTravelled += distance;
            Vector2 position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            _trasform.DOMove(position, time).OnComplete(action);
        }

        public void MoveDistance(float distance, float time)
        {
            MoveDistance(distance, time, delegate () { });
        }
        #endregion

        public void OnDisable()
        {
            ConnectWithChain = null;
        }

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (!_trasform.CompareTag("Edge") || !coll.CompareTag("Edge"))
                return;

            if (ConnectWithChain == null) {
                return;
            }
            PathFollower collPath = coll.GetComponent<PathFollower>();
            ConnectWithChain(this, collPath);
        }
    }
}
