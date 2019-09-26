using System;

using UnityEngine;
using Entitas;

/// <summary>
/// Счётчик времени. Имеет также событие которое должно отработать по окончанию счётчика
/// </summary>
[Game]
public class CounterComponent : IComponent
{
    public float value;
    public Action postAction;
}
