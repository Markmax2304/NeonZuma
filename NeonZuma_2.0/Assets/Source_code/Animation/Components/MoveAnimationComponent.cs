using System;

using UnityEngine;
using Entitas;

[Game]
public class MoveAnimationComponent : IComponent
{
    public float duration;
    public Vector3 target;
    public Action postAction;
}
