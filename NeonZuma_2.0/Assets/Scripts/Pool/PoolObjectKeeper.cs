using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PoolObjectKeeper
{
    private static int id = 0;
    private int maxCount;
    private Transform parent;
    private GameObject prefab;
    private List<PoolingObject> pool;

    public PoolObjectKeeper(GameObject prefab, Transform parent, int count)
    {
        this.prefab = prefab;
        this.parent = parent;
        maxCount = count;
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
        GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        StringBuilder name = new StringBuilder("Ball_").Append(id++);
        obj.name = name.ToString();                                              // just test, will delete
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
        for (int i = 0; i < pool.Count; i++) {
            if (pool[i].IsAccess) {
                return pool[i];
            }
        }

        AddNewObject(maxCount);
        return pool[pool.Count - 1];
    }
    #endregion
}
