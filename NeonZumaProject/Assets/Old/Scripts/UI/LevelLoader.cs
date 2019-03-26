using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Core;

namespace UI
{
    public class LevelLoader : MonoBehaviour
    {
        public int levelIndex;

        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnEnable()
        {
            //Debug.Log("Subscibe " + name);
            button.onClick.AddListener(delegate() { LevelLoaderManager.instance.LoadLevel(levelIndex); });
        }

        public void OnDisable()
        {
            //Debug.Log("Unsubscibe " + name);
            button.onClick.RemoveAllListeners();
        }
    }
}
