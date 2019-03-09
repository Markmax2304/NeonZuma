using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Core.Pool;

namespace Core
{
    public class Projectile : MonoBehaviour
    {
        public event BallCollisionHandler CollisionBalls;

        float lifeTime = 1f;
        float speed = 0f;
        Vector3 direction = Vector3.zero;
        bool isMove = false;

        Transform _transform;

        void Start()
        {
            _transform = transform;
        }

        void FixedUpdate()
        {
            //turn on - if player shooted;  turn off - if ball is collided with other balls
            if (isMove) {
                _transform.position += direction * speed * Time.fixedDeltaTime;
            }
        }

        public void SetMoveDirection(float speed, Vector2 dir)
        {
            this.speed = speed;
            direction = dir;
            isMove = true;
        }

        public void Die()
        {
            if (isMove) {
                isMove = false;
                GetComponent<PoolingObject>().ReturnToPool();
            }
        }

        public void OnDisable()
        {
            CollisionBalls = null;
        }

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (!isMove || (!coll.CompareTag("Chain") && !coll.CompareTag("Edge")))
                return;
            
            isMove = false;
            if(CollisionBalls == null) {
                Debug.Log("Projectile event equal null: " + name);
                return;
            }
            CollisionBalls(GetComponent<PathFollower>(), coll.GetComponent<PathFollower>());
        }
    }
}
