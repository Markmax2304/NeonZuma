using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public enum TypeObjectPool { Ball };

    public class PoolManager : MonoBehaviour
    {
        public static PoolManager instance;         //протестить на переходе между сценами

        Dictionary<TypeObjectPool, PoolObjectKeeper> pools;
        [SerializeField] PoolInfo[] poolInfo;

        void Awake()
        {
            if(instance == null) {
                instance = this;
            }
            else {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);

            pools = new Dictionary<TypeObjectPool, PoolObjectKeeper>();
            for (int i = 0; i < poolInfo.Length; i++) {
                Transform parent = poolInfo[i].parent == null ? transform : poolInfo[i].parent;
                pools.Add(poolInfo[i].type, new PoolObjectKeeper(poolInfo[i].prefab, parent, poolInfo[i].count));
            }

            InitializePools();
        }

        void InitializePools()
        {
            foreach (PoolObjectKeeper pool in pools.Values) {
                pool.InitializePool();
            }
        }

        public PoolObjectKeeper GetObjectPoolKeeper(TypeObjectPool type)
        {
            return pools[type];
        }

        public void ReturnAllObjects()
        {
            foreach(KeyValuePair<TypeObjectPool, PoolObjectKeeper> pool in pools) {
                pool.Value.ResetPool();
            }
        }

        [System.Serializable]
        public struct PoolInfo
        {
            public TypeObjectPool type;
            public int count;
            public GameObject prefab;
            public Transform parent;
        }
    }
}
