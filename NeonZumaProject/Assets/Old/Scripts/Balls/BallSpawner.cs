using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Core
{
    public class BallSpawner : MonoBehaviour
    {
        public CommonHandler spawnBall;
        public BallHandler removeBall;

        Transform _transform;

        void Awake()
        {
            _transform = transform;
            PathCreator pathCreator = GetComponentInParent<PathCreator>();
            transform.position = pathCreator.path.GetPointAtDistance(0f, EndOfPathInstruction.Stop);
        }

        public Vector3 GetPosition()
        {
            return _transform.position;
        }

        public void OnTriggerEnter2D(Collider2D coll)
        {
            //Debug.Log("enter for remove " + coll.tag);
            if (coll.CompareTag("Chain") || coll.CompareTag("Edge")) {
                if (removeBall != null) {
                    removeBall(coll.GetComponent<PathFollower>());
                }
            }
        }

        public void OnTriggerExit2D(Collider2D coll)
        {
            //Debug.Log("exit " + coll.tag);
            if (coll.CompareTag("Chain") || coll.CompareTag("Edge")) {
                if (spawnBall != null) {
                    spawnBall();
                }
            }
        }
    }
}
