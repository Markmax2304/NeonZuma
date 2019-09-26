using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Текущая нормальная скорость на уровне
/// Выделен отдельный компонент, чтобы можно было легко менять и не требовалось запоминать в отдельную переменную
/// </summary>
[Global, Unique]
public class CurrentNormalSpeedComponent : IComponent
{
    public float value;
}
