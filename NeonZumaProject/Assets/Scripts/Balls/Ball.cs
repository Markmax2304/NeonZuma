using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Core
{
    public class Ball : MonoBehaviour
    {
        public event BallCollisionHandler OnCollisionBall = delegate (PathFollower ball, PathFollower coll) { };

        float speed = 0f;
        Vector3 direction = Vector3.zero;
        bool isMove = false;

        SpriteRenderer spriteRenderer;
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

        public void SetColor(Color color)
        {
            if (spriteRenderer == null) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            spriteRenderer.color = color;
        }

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (!isMove || coll.tag.CompareTo("Chain") != 0)
                return;

            isMove = false;
            OnCollisionBall(GetComponent<PathFollower>(), coll.GetComponent<PathFollower>());
        }
    }
}
