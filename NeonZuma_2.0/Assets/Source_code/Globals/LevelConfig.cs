using UnityEngine;
using Entitas.CodeGeneration.Attributes;

[Game, Unique, CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptObject/AddLevelConfig")]
public class LevelConfig : ScriptableObject
{
    public GameObject[] pathCreatorPrefabs;
    public GameObject playerPrefab;

    [Header("Ball fields"), Space]
    public ColorInfo[] colors;
    public float followSpeed = .5f;

    public float offsetBetweenBalls = .36f;

    [Header("Chain fields"), Space]
    public int minLengthSeries = 1;
    public int maxLengthSeries = 6;

    [Header("Player fields"), Space]
    public float rotateSpeed = 1f;
    public float rechargeTime = 1f;

    [Header("Projectile fields"), Space]
    public float forceSpeed = 5f;
}

[System.Serializable]
public struct ColorInfo
{
    public ColorBall type;
    public Color color;
}
