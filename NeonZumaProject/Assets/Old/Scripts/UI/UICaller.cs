using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;

namespace UI
{
    public class UICaller : MonoBehaviour
    {
        Button button;
        bool isActive = false;

        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);
            DisableSelf();
            LevelEventSystem.instance.Subscribe(LevelEventType.Start, EnableSelf);
            LevelEventSystem.instance.Subscribe(LevelEventType.GameOver, DisableSelf);
        }

        public void OnDestroy()
        {
            LevelEventSystem.instance.Unsubscribe(LevelEventType.Start, EnableSelf);
            LevelEventSystem.instance.Unsubscribe(LevelEventType.GameOver, DisableSelf);
        }

        void Click()
        {
            isActive = !isActive;
            if (isActive) {
                LevelEventSystem.instance.InvokeEvent(LevelEventType.PauseEnter);
            }
            else {
                LevelEventSystem.instance.InvokeEvent(LevelEventType.PauseExit);
            }
        }

        void EnableSelf()
        {
            button.interactable = true;
        }

        void DisableSelf()
        {
            button.interactable = false;
        }
    }
}
