using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Pool;

namespace Core
{
    public class BallSpawner : MonoBehaviour
    {
        PoolObjectKeeper poolKeeper;

        void Start()
        {
            poolKeeper = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
        }

        public PathFollower SpawnBall()
        {
            PoolingObject obj = poolKeeper.RealeseObject();
            return obj.GetComponent<PathFollower>();
        }
    }
}
