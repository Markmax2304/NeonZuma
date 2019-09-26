using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Общий счётчик очков на уровне
/// </summary>
[Manage, Unique]
public class TotalScoreComponent : IComponent
{
    public int value;
}
