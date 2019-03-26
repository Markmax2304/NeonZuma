using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace UI
{
    public class UIElement : MonoBehaviour
    {
        GameObject _gameObject;

        public void Awake()
        {
            _gameObject = gameObject;
            _gameObject.SetActive(false);

            LevelEventSystem.instance.Subscribe(LevelEventType.PauseEnter, Show);
            LevelEventSystem.instance.Subscribe(LevelEventType.PauseExit, Hide);
            LevelEventSystem.instance.Subscribe(LevelEventType.GameOver, Show);
        }

        public void OnDestroy()
        {
            LevelEventSystem.instance.Unsubscribe(LevelEventType.PauseEnter, Show);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.PauseExit, Hide);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.GameOver, Show);
        }

        public void Show()
        {
            if (!_gameObject.activeInHierarchy) {
                _gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (_gameObject.activeInHierarchy) {
                _gameObject.SetActive(false);
            }
        }
    }
}
