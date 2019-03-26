using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace UI
{
    public class LevelLoaderManager : MonoBehaviour
    {
        public static LevelLoaderManager instance;

        void Awake()
        {
            if(instance == null) {
                instance = this;
            }
            else {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void LoadLevel(int index)
        {
            // animate transition
            PoolManager.instance.ReturnAllObjects();
            SceneManager.LoadScene(index);
        }
    }
}
