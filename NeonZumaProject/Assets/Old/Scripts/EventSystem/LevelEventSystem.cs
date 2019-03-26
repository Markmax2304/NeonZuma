using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class LevelEventSystem : MonoBehaviour
    {
        public static LevelEventSystem instance;

        Dictionary<LevelEventType, UnityEvent> eventDictionary;

        void Awake()
        {
            /*if(instance == null) {
                instance = this;
            }
            else {
                Destroy(gameObject);
            }*/

            instance = this;
            eventDictionary = new Dictionary<LevelEventType, UnityEvent>();

            //DontDestroyOnLoad(gameObject);
        }

        public void Subscribe(LevelEventType eventType, UnityAction handler)
        {
            UnityEvent thisEvent;
            if(eventDictionary.TryGetValue(eventType, out thisEvent)) {
                thisEvent.AddListener(handler);
            }
            else {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(handler);
                eventDictionary.Add(eventType, thisEvent);
            }
        }

        public void Unsubscribe(LevelEventType eventName, UnityAction handler)
        {
            UnityEvent thisEvent;
            if (eventDictionary.TryGetValue(eventName, out thisEvent)) {
                thisEvent.RemoveListener(handler);
            }
        }

        public void InvokeEvent(LevelEventType eventName)
        {
            UnityEvent thisEvent;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
                thisEvent.Invoke();
            }
        }
    }
}
