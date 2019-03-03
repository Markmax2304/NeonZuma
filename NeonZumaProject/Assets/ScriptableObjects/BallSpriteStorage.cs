using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "BallTypes", menuName = "ScriptObject/AddBallTypes", order = 52)]
    public class BallTypesStorage : ScriptableObject
    {
        public List<BallInfo> ballTypes;
    }

    [System.Serializable]
    public struct BallInfo
    {
        public BallType type;
        //public Sprite sprite;
        public Color color;     //test
    }
}
