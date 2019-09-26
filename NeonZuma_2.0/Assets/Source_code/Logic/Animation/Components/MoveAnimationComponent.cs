using System;

using UnityEngine;
using Entitas;

/// <summary>
/// Данные для запуска анимации движения у объекта
/// Также есть пост событие, которое выставляет флаг об окончании анимации
/// </summary>
[Game]
public class MoveAnimationComponent : IComponent
{
    public float duration;
    public Vector3 target;
    public Action postAction;
}
