using System;

using UnityEngine;
using Entitas;

[Game]
public class MoveAnimationComponent : IComponent
{
    public float duration;
    public Vector3 target;
    // TODO: maybe add ease type, if I need
    public Action postAction;
}
