using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas.CodeGeneration.Attributes;
using PathCreation;

[Game, Unique, CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptObject/AddLevelConfig")]
public class LevelConfig : ScriptableObject
{
    public GameObject[] pathCreatorPrefabs;

    public float followSpeed = .5f;

    public float offsetBetweenBalls = .36f;
}
