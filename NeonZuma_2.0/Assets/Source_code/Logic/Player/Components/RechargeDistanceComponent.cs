using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Дистанция от центра игрока до положения снаряда
/// </summary>
[Global, Unique]
public class RechargeDistanceComponent : IComponent
{
    public Vector3 value;
}
