using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingObject : MonoBehaviour
{
    private GameObject _gameObject;
    private Transform parent;

    public bool IsAccess {
        get { return !_gameObject.activeInHierarchy; }
    }

    void Awake()
    {
        _gameObject = gameObject;
        parent = _gameObject.transform.parent;
    }

    public void ReturnToPool()
    {
        parent = PoolManager.instance.transform;
        _gameObject.SetActive(false);
    }
}
