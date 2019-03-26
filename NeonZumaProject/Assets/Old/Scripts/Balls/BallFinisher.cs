using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Core
{
    public class BallFinisher : MonoBehaviour
    {
        bool isGameOver = false;

        void Awake()
        {
            PathCreator pathCreator = GetComponentInParent<PathCreator>();
            transform.position = pathCreator.path.GetPoint(1f, EndOfPathInstruction.Stop);
        }

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (!coll.CompareTag("Chain") && !coll.CompareTag("Edge"))
                return;

            coll.GetComponent<PoolingObject>().ReturnToPool();
            if (!isGameOver) {
                LevelEventSystem.instance.InvokeEvent(LevelEventType.GameOver);
                isGameOver = true;
            }
        }
    }
}
