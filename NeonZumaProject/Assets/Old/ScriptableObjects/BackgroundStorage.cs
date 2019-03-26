using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Backgrounds", menuName = "ScriptObject/AddBackgroundsStorage", order = 52)]
    public class BackgroundStorage : ScriptableObject
    {
        public List<BackgroundInfo> backgrounds;
    }

    [System.Serializable]
    public struct BackgroundInfo
    {
        public BackgroundType type;
        public Sprite sprite;
    }
}
