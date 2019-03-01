using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Pool
{
    public class PoolObjectKeeper
    {
        int maxCount;
        Transform parent;
        GameObject prefab;
        List<PoolingObject> pool;

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

        public PoolingObject RealeseObject()
        {
            PoolingObject obj = null;
            for (int i = 0; i < pool.Count; i++) {
                if (!pool[i].IsAccess) {
                    obj = pool[i];
                    break;
                }
            }

            if (obj == null) {
                AddNewObject(maxCount);
                obj = pool[pool.Count - 1];
            }

            obj.gameObject.SetActive(true);
            return obj;
        }


        void AddNewObject()
        {
            GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
            obj.SetActive(false);
            pool.Add(obj.GetComponent<PoolingObject>());
        }

        void AddNewObject(int amount)
        {
            for (int i = 0; i < amount; i++) {
                AddNewObject();
            }
        }
    }
}
