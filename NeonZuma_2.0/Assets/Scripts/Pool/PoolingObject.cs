using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingObject : MonoBehaviour
{
    GameObject _gameObject;

    public bool IsAccess {
        get { return !_gameObject.activeInHierarchy; }
    }

    void Awake()
    {
        _gameObject = gameObject;
    }

    public void ReturnToPool()
    {
        _gameObject.transform.parent = PoolManager.instance.transform;
        _gameObject.SetActive(false);
    }
}
