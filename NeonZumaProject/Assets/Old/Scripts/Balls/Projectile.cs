using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Projectile : MonoBehaviour
    {
        public event BallCollisionHandler CollisionBalls;

        float lifeTime = 1f;
        float speed = 0f;
        Vector3 direction = Vector3.zero;
        bool isEnable = false;

        Transform _transform;
        Rigidbody2D _rig;

        void Start()
        {
            _transform = transform;
            _rig = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            //turn on - if player shooted;  turn off - if ball is collided with other balls
            if (isEnable) {
                _rig.MovePosition(_transform.position + direction * speed * Time.fixedDeltaTime);
            }
        }

        public void SetMoveDirection(float _speed, Vector2 dir)
        {
            isEnable = true;
            speed = _speed;
            direction = dir;
        }

        public void Die()
        {
            if (isEnable) {
                isEnable = false;
                GetComponent<PoolingObject>().ReturnToPool();
            }
        }

        public void OnDisable()
        {
            CollisionBalls = null;
        }

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (!isEnable || (!coll.CompareTag("Chain") && !coll.CompareTag("Edge")))
                return;
            
            if(CollisionBalls == null) {
                Debug.Log("Projectile event equal null: " + name);
                return;
            }
            isEnable = false;
            CollisionBalls(GetComponent<PathFollower>(), coll.GetComponent<PathFollower>());
        }
    }
}
