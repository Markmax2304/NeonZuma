using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Pool
{
    public class PoolingObject : MonoBehaviour
    {
        GameObject _gameObject;

        public bool IsAccess {
            get { return _gameObject.activeInHierarchy; }
        }

        void Awake()
        {
            _gameObject = gameObject;
        }

        public void ReturnToPool()
        {
            _gameObject.SetActive(false);
            transform.parent = PoolManager.instance.transform;
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }
    }
}
