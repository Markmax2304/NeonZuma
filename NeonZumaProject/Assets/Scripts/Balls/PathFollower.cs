using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using DG.Tweening;
using Core.Pool;

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
        Ball ball;
        Transform _transform;
        Animator animator;
        Collider2D _collider;

        void Awake()
        {
            _transform = transform;
            animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
        }

        public void Initialize(PathCreator path, float distance = 0)
        {
            id = nextId++;
            tag = "Chain";
            pathCreator = path;
            ball = GetComponent<Ball>();
            distanceTravelled = distance;
        }

        public void DestroyBall(TweenCallback action)
        {
            SetAnimateSpeed(0f);
            _collider.enabled = false;
            transform.DOScale(.1f, .15f).OnComplete(delegate() {
                action();
                GetComponent<PoolingObject>().ReturnToPool();
                _transform.localScale = Vector3.one * .4f;
                _collider.enabled = true;
                SetAnimateSpeed(1f);
            });
            // particle system
            
        }

        public void DestroyBall()
        {
            DestroyBall(delegate () { });
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
                _transform.tag = "Edge";
            else
                _transform.tag = "Chain";
        }

        public void SetAnimateSpeed(float speed)
        {
            float direct = speed >= 0 ? 1f : -1f;
            animator.speed = Mathf.Abs(speed);
        }
        #endregion

        #region Movement

        public void MoveUpdate(float delta)
        {
            if (pathCreator != null) {
                distanceTravelled += delta;
                _transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                Vector3 direction = pathCreator.path.GetDirectionAtDistance(distanceTravelled, endOfPathInstruction);
                Quaternion rotate = Quaternion.FromToRotation(Vector3.down, direction);
                _transform.rotation = rotate;
            }
        }

        public void MoveDistanceToPoint(float distancePoint, float speed, TweenCallback action)
        {
            float time = Mathf.Abs(distancePoint - distanceTravelled) / speed;
            distanceTravelled = distancePoint;
            Vector2 position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            _transform.DOMove(position, time).OnComplete(action);
        }

        public void MoveDistanceToPoint(float distancePoint, float speed)
        {
            MoveDistanceToPoint(distancePoint, speed, delegate () { });
        }

        public void MoveDistance(float distance, float time, TweenCallback action)
        {
            distanceTravelled += distance;
            Vector2 position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            _transform.DOMove(position, time).OnComplete(action);
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
            if (!_transform.CompareTag("Edge") || !coll.CompareTag("Edge"))
                return;

            if (ConnectWithChain == null) {
                return;
            }
            PathFollower collPath = coll.GetComponent<PathFollower>();
            ConnectWithChain(this, collPath);
        }
    }
}
