using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Core
{
    public class PlayerController : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] Transform gunTransform;
        [SerializeField] Transform nextGunTransform;
        BallsController controller;
        BallRandom randomizator;
        [SerializeField] float speedRotate = 20f;
        [SerializeField] float speedShooting = 10f;
        [SerializeField] float timeOutBetweenShot = .3f;

        Transform _transform;
        PoolObjectKeeper poolKeeper;
        Transform ball;
        Transform nextBall;

        bool isEnable;
        bool isPressed = false;

        public bool IsEnable {
            set { isEnable = value; }
        }

        void Start()
        {
            _transform = transform;
            poolKeeper = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
            controller = FindObjectOfType<BallsController>();
            randomizator = controller.randomizator;

            isEnable = false;
        }

        public void OnEnable()
        {
            LevelEventSystem.instance.Subscribe(LevelEventType.Start, PrepareToGame);
            LevelEventSystem.instance.Subscribe(LevelEventType.Win, FinishGame);
            LevelEventSystem.instance.Subscribe(LevelEventType.GameOver, FinishGame);
            LevelEventSystem.instance.Subscribe(LevelEventType.PauseEnter, Disable);
            LevelEventSystem.instance.Subscribe(LevelEventType.PauseExit, Enable);
        }

        public void OnDisable()
        {
            LevelEventSystem.instance.Unsubscribe(LevelEventType.Start, PrepareToGame);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.Win, FinishGame);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.GameOver, FinishGame);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.PauseEnter, Disable);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.PauseExit, Enable);
        }

        #region Events

        void PrepareToGame()
        {
            isEnable = true;
            ball = GetBall(gunTransform);
            nextBall = GetBall(nextGunTransform);
        }

        void FinishGame()
        {
            isEnable = false;
            /*ball.GetComponent<PoolingObject>().ReturnToPool();
            ball = null;
            nextBall.GetComponent<PoolingObject>().ReturnToPool();
            nextBall = null;*/
        }

        void Enable()
        {
            isEnable = true;
        }

        void Disable()
        {
            isEnable = false;
        }
        #endregion

        void Update()
        {
            if (!isEnable)
                return;

            if (Input.GetMouseButtonDown(0)) {
                PointerEventData data;
#if UNITY_EDITOR
                data = GetPointerData(-1);
#endif
#if UNITY_ANDROID
                //data = GetPointerData(Input.touches[0].fingerId);
#endif
                
                if (data == null) {
                    return;
                }

                if(data.pointerEnter.layer == 5) {
                    isPressed = false;
                }
                else {
                    isPressed = true;
                }
            }

            if (isPressed) {
                if (Input.GetMouseButtonUp(0)) {
                    RotateFollowMouse();
                    ShootBall();
                }
                else if (Input.GetMouseButton(0)) {
                    RotateFollowMouseClamp();
                }
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (isEnable) {
                SwapCurrentBalls();
            }
        }

        PointerEventData GetPointerData(int id)
        {
            StandaloneModule currentInput = EventSystem.current.currentInputModule as StandaloneModule;
            if (currentInput == null) {
                return null;
            }

            return currentInput.GetPointerEventData(id);

        }

#region Control methods (Shoot, Rotate, Swap)

        void ShootBall()
        {
            Vector2 direction = CalculateDirectionByMouse();
            ball.parent = null;
            Projectile obj = ball.GetComponent<Projectile>();
            obj.SetMoveDirection(speedShooting, direction);
            obj.CollisionBalls += controller.OnCollisionBalls;
            obj.tag = "Projectile";

            ChargeNewBall();
        }

        void ChargeNewBall()
        {
            ball = nextBall;
            isEnable = false;
            ball.DOMove(gunTransform.position, timeOutBetweenShot).OnComplete(delegate () { isEnable = true; });
            nextBall = GetBall(nextGunTransform);
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

        Transform GetBall(Transform parentPosition)
        {
            Transform ball = poolKeeper.RealeseObject(parentPosition.position).transform;
            ball.GetComponent<PathFollower>().SetType(randomizator.GetSingleBall());
            ball.rotation = parentPosition.rotation;
            ball.up = -ball.up;
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
