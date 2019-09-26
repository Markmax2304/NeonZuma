using System;

using UnityEngine;
using Entitas;

/// <summary>
/// Данные для запуска анимации масштабирования объекта
/// Также есть пост событие, которое выставляет флаг об окончании анимации
/// </summary>
[Game]
public class ScaleAnimationComponent : IComponent
{
    public float duration;
    public float targetScale;
    public Action postAction;
}
