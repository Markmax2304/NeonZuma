using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Скорость с которой должен лететь снаряд
/// </summary>
[Global, Unique]
public class ForceSpeedComponent : IComponent
{
    public float value;
}
