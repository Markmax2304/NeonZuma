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
        public BallType type;
        public int id;
        static int nextId = 0;

        PathCreator pathCreator;
        PoolingObject _poolingObj;
        Transform _transform;
        Rigidbody2D _rig;
        Animator _animator;
        Collider2D _collider;
        SpriteRenderer _spriteRend;

        void Awake()
        {
            _transform = transform;
            _poolingObj = GetComponent<PoolingObject>();
            _rig = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
            _spriteRend = GetComponent<SpriteRenderer>();
        }

        public void Initialize(PathCreator path, float distance = 0)
        {
            id = nextId++;
            tag = "Chain";      //excess?
            pathCreator = path;
            distanceTravelled = distance;
        }

        public void SetType(BallInfo info)
        {
            _spriteRend.color = info.color;
            type = info.type;
        }

        public void DestroyBall(TweenCallback action)
        {
            SetAnimateSpeed(0f);
            tag = "Untagged";
            _collider.enabled = false;
            transform.DOScale(.1f, .15f).OnComplete(delegate() {
                action();
                _poolingObj.ReturnToPool();
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

        public void OnDisable()
        {
            ConnectWithChain = null;
        }

        #region Property

        public float Distance {
            get { return distanceTravelled; }       
            set { distanceTravelled = value; }
        }

        public Vector2 GetPosition()
        {
            return _transform.position;
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
            _animator.SetFloat("direct", direct);
            _animator.speed = Mathf.Abs(speed);
        }
        #endregion

        #region Movement

        public void MoveUpdate(float delta)
        {
            if (pathCreator != null) {
                distanceTravelled += delta;
                _rig.MovePosition(pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction));
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
