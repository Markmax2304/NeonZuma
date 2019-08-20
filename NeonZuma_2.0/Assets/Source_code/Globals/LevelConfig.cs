using UnityEngine;
using Entitas.CodeGeneration.Attributes;

[Game, Unique, CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptObject/AddLevelConfig")]
public class LevelConfig : ScriptableObject
{
    public GameObject[] pathCreatorPrefabs;
    public GameObject playerPrefab;

    [Header("Ball fields"), Space]
    public ColorInfo[] colors;
    [Range(0, 5)]
    public float followSpeed = .5f;
    public float insertDuration = .25f;
    public float destroyAnimationDuration = .25f;

    public float ballDiametr = .36f;
    public float minScaleSize = .05f;
    public Vector3 normalScale = new Vector3(.4f, .4f, .4f);

    [Header("Chain fields"), Space]
    public int minLengthSeries = 1;
    public int maxLengthSeries = 6;

    [Header("Player fields"), Space]
    public float rotateSpeed = 1f;
    public float rechargeTime = 1f;

    [Header("Projectile fields"), Space]
    public float forceSpeed = 5f;
    public string[] projectileTags;
}

[System.Serializable]
public struct ColorInfo
{
    public ColorBall type;
    public Color color;
}
