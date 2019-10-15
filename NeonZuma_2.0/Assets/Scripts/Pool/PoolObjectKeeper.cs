using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PoolObjectKeeper
{
    private const string underDash = "_";

    private static int id = 0;
    private string name;
    private int maxCount;
    private Transform _parent;
    private GameObject _prefab;
    private List<PoolingObject> pool;

    public PoolObjectKeeper(GameObject prefab, Transform parent, int count, string objectName)
    {
        _prefab = prefab;
        _parent = parent;
        maxCount = count;
        name = objectName;
        pool = new List<PoolingObject>();
    }

    public void InitializePool()
    {
        InitializePool(maxCount);
    }

    public void InitializePool(int count)
    {
        AddNewObject(count);
    }

    public void ResetPool()
    {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].IsAccess) {
                pool[i].ReturnToPool();
            }
        }
    }

    public PoolingObject RealeseObject()
    {
        PoolingObject obj = GetFreeObject();

        obj.gameObject.SetActive(true);
        return obj;
    }

    public PoolingObject RealeseObject(Vector2 position)
    {
        PoolingObject obj = GetFreeObject();

        obj.transform.position = position;
        obj.gameObject.SetActive(true);
        return obj;
    }

    public PoolingObject RealeseObject(Vector2 position, Quaternion rotation)
    {
        PoolingObject obj = GetFreeObject();

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.gameObject.SetActive(true);
        return obj;
    }

    public PoolingObject RealeseObject(Vector2 position, Quaternion rotation, Vector3 scale)
    {
        PoolingObject obj = GetFreeObject();

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = scale;
        obj.gameObject.SetActive(true);
        return obj;
    }

    #region Private
    private void AddNewObject()
    {
        GameObject obj = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _parent);
        StringBuilder sb = new StringBuilder(name).Append(underDash).Append(id++);
        // TODO: just test, will delete
        obj.name = sb.ToString();
        obj.SetActive(false);
        pool.Add(obj.GetComponent<PoolingObject>());
    }

    private void AddNewObject(int amount)
    {
        for (int i = 0; i < amount; i++) {
            AddNewObject();
        }
    }

    private PoolingObject GetFreeObject()
    {
        int count = pool.Count;
        for (int i = 0; i < count; i++) {
            if (pool[i].IsAccess) {
                return pool[i];
            }
        }

        AddNewObject(maxCount);
        return pool[count + 1];
    }
    #endregion
}
