using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// <summary>
/// Трансформ компонент из Юнити
/// </summary>
[Game]
public class TransformComponent : IComponent
{
    public Transform value;
}
