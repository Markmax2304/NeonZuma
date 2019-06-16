using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas.CodeGeneration.Attributes;
using PathCreation;

[Game, Unique, CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptObject/AddLevelConfig")]
public class LevelConfig : ScriptableObject
{
    public GameObject[] pathCreatorPrefabs;
    public GameObject playerPrefab;

    [Header("Ball fields"), Space]
    public float followSpeed = .5f;

    public float offsetBetweenBalls = .36f;

    [Header("Player fields"), Space]
    public float rotateSpeed = 1f;
}
