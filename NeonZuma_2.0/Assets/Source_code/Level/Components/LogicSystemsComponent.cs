using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Компонент хранит системы, что выполняются на игровом уровне
/// В принципе, этот компонент и есть игровой уровень, в какой-то степени
/// </summary>
[Manage, Unique]
public class LogicSystemsComponent : IComponent
{
    public Systems value;
}
