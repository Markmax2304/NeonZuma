using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Core.Pool;

namespace Core.Player
{
    public class PlayerController : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] Transform gunTransform;
        [SerializeField] Transform nextGunTransform;
        BallsController controller;
        public float speedRotate = 1f;
        public float speedShooting = 10f;

        Transform _transform;
        PoolObjectKeeper poolKeeper;
        Transform ball;
        Transform nextBall;

        bool isEnable = true;
        bool isPressed = false;
        bool isSwapped = false;

        public bool Enable {
            set { isEnable = value; }
        }

        void Start()
        {
            _transform = transform;
            poolKeeper = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
            controller = FindObjectOfType<BallsController>();

            // initialize balls
            ball = GetNewBall(gunTransform);
            nextBall = GetNewBall(nextGunTransform);
        }

        void Update()
        {
            // если мы хотим отключить какие-либо действия на контроллере, просто отключаем это свойство
            if (!isEnable)
                return;

            // ещё одна проверка на нажатие, ибо мы не знаем когда сработает метод Update, а когда событие клика
            if (isPressed) {
                isPressed = false;
                return;
            }

            // логика свапа - после нажатия на объект ничего не должно происходить
            if (isSwapped) {
                if (Input.GetMouseButtonDown(0)) {
                    isSwapped = false;
                }
                return;
            }

            if (Input.GetMouseButtonUp(0)) {
                RotateFollowMouse();
                ShootBall();
            }
            else if (Input.GetMouseButton(0)) {
                RotateFollowMouseClamp();
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            SwapCurrentBalls();
            isPressed = true;
            isSwapped = true;
        }

        #region Control methods (Shoot, Rotate, Swap)

        void ShootBall()
        {
            Vector2 direction = CalculateDirectionByMouse();
            ball.parent = null;
            Ball obj = ball.GetComponent<Ball>();
            obj.SetMoveDirection(speedShooting, direction);
            obj.OnCollisionBall += controller.InsertBallToChain;

            ball = GetNewBall(gunTransform);
            SwapCurrentBalls();
        }

        void SwapCurrentBalls()
        {
            Transform tempBall = ball;
            ball = nextBall;
            nextBall = tempBall;

            ball.position = gunTransform.position;
            nextBall.position = nextGunTransform.position;
        }

        void RotateFollowMouseClamp()
        {
            Vector2 direction = CalculateDirectionByMouse();
            _transform.up = Vector2.Lerp(_transform.up, direction, Time.deltaTime * speedRotate);
        }

        void RotateFollowMouse()
        {
            Vector2 direction = CalculateDirectionByMouse();
            _transform.up = direction;
        }
        #endregion

        #region Extra Help Methods

        Transform GetNewBall(Transform parentPosition)
        {
            Transform ball = poolKeeper.RealeseObject().transform;
            ball.position = parentPosition.position;
            ball.parent = _transform;
            return ball;
        }

        Vector2 CalculateDirectionByMouse()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerPos = _transform.position;

            Vector2 direction = mousePos - playerPos;
            direction.Normalize();
            return direction;
        }
        #endregion
    }
}
