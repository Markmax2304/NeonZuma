using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "GeneralLevelSettings", menuName = "ScriptObject/AddGeneralLevelSettings", order = 52)]
    public class GeneralLevelSettings : ScriptableObject
    {
        [Header("Speed data")]
        public float defeatSpeed = 20f;
        public float startSpeed = 5f;
        public float middleSpeed = .5f;
        public float endSpeed = .25f;
        [Range(0, 100)] public int startLengthByPercent = 30;
        [Range(0, 100)] public int endLengthByPercent = 90;

        [Header("Back tracking and attraction")]
        public float backTrackSpeed = 7f;
        public float maxAttractiveSpeed = 5f;
        [Range(0, 1)] public float decreaseRate = .9f;

        [Header("Animate variable")]
        public float offsetBetweenBalls = .36f;
        public float animSpeed = 3f;                            // for animation of connection and lining
        public float timeForInsertingAnimation = .15f;          // time for animation of inserting
    }
}
