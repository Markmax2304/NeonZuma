using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;         //протестить на переходе между сценами

    private Dictionary<TypeObjectPool, PoolObjectKeeper> pools;
    [SerializeField] private PoolInfo[] poolInfo;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        InitializePools();
    }

    #region Public Methods
    public PoolObjectKeeper GetObjectPoolKeeper(TypeObjectPool type)
    {
        return pools[type];
    }

    public void ReturnAllObjects()
    {
        foreach (KeyValuePair<TypeObjectPool, PoolObjectKeeper> pool in pools) {
            pool.Value.ResetPool();
        }
    }
    #endregion

    #region Private Methods
    private void InitializePools()
    {
        pools = new Dictionary<TypeObjectPool, PoolObjectKeeper>();
        for (int i = 0; i < poolInfo.Length; i++)
        {
            Transform parent = poolInfo[i].parent == null ? transform : poolInfo[i].parent;
            pools.Add(poolInfo[i].type, new PoolObjectKeeper(poolInfo[i].prefab, parent, poolInfo[i].count, poolInfo[i].type.ToString()));
        }

        foreach (var pool in pools)
        {
            pool.Value.InitializePool();
        }
    }
    #endregion

    #region Nested Types
    [System.Serializable]
    public struct PoolInfo
    {
        public TypeObjectPool type;
        public int count;
        public GameObject prefab;
        public Transform parent;
    }
    #endregion
}
