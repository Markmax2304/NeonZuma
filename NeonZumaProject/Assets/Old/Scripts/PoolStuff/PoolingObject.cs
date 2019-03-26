using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class PoolingObject : MonoBehaviour
    {
        GameObject _gameObject;
        Transform _transform;

        public bool IsAccess {
            get { return !_gameObject.activeInHierarchy; }
        }

        void Awake()
        {
            _gameObject = gameObject;
            _transform = transform;
        }

        public void ReturnToPool()
        {
            if (!IsAccess) {
                _gameObject.SetActive(false);
                _transform.tag = "Untagged";
                _transform.parent = PoolManager.instance.transform;
                _transform.position = PoolManager.instance.transform.position;
            }
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }
    }
}
