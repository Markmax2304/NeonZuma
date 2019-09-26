using UnityEngine;
using Entitas;

/// <summary>
/// Направление в котором должен лететь снаряд
/// </summary>
[Game]
public class ForceComponent : IComponent
{
    public Vector2 value;
}
