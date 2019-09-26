using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Комбо откатов цепей назада подряд
/// </summary>
[Manage, Unique]
public class MoveBackComboComponent : IComponent
{
    public int value;
}
