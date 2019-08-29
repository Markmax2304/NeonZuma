using System;

using UnityEngine;
using Entitas;

[Game]
public class CounterComponent : IComponent
{
    public float value;
    public Action postAction;
}
