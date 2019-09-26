using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// <summary>
/// Позиция на игровом поле, где произошёл touch
/// </summary>
[Input]
public class TouchPositionComponent : IComponent
{
    public Vector2 value;
}
