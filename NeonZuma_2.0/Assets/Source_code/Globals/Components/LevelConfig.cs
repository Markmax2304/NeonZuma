using UnityEngine;
using Entitas.CodeGeneration.Attributes;

[Global, Unique, CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptObject/AddLevelConfig")]
public class LevelConfig : ScriptableObject
{
    public GameObject[] pathCreatorPrefabs;
    public GameObject playerPrefab;

    [Header("Ball fields"), Space]
    public ColorInfo[] colors;
    public float followSpeed = .5f;
    public float insertDuration = .25f;
    public float destroyAnimationDuration = .25f;
    [Space]
    public float ballDiametr = .36f;
    public float minScaleSize = .05f;
    public Vector3 normalScale = new Vector3(.4f, .4f, .4f);

    [Header("Chain fields"), Space]
    public int minLengthSeries = 1;
    public int maxLengthSeries = 6;
    [Space]
    public float alignBallAnimationDuration = 5f;
    [Space]
    public float moveBackSpeed = 3f;
    public float moveBackDuration = .5f;
    public float increaseMoveBackFactor = .25f;
    public float gravitateSpeed = 2f;
    [Space]
    public float startSpeed = 5f;
    public float startDuration = 4f;
    [Space]
    public float normalSpeedLengthPercent = .75f;
    [Space]
    public float gameOverSpeed = 12f;

    [Header("Player fields"), Space]
    public float rotateSpeed = 1f;
    public float rechargeTime = 1f;

    [Header("Projectile fields"), Space]
    public float forceSpeed = 5f;

    [Header("Score fields"), Space]
    public int scorePerBall = 10;
    public int AmountBallAfterApplyRowCombo = 3;

    [Header("Ability fields"), Space]
    public float freezeDuration = 3f;
    [Space]
    public float rollbackSpeed = 2f;
    public float rollbackDuration = 3f;
    [Space]
    public float pointerDuration = 3f;
    public float pointerShootSpeed = 10f;
    [Space]
    public float explosionRadius = 1.5f;
}

[System.Serializable]
public struct ColorInfo
{
    public ColorBall type;
    public Color color;
}
