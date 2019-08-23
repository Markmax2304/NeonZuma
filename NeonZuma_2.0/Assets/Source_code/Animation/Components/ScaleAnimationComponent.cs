using System;

using UnityEngine;
using Entitas;

[Game]
public class ScaleAnimationComponent : IComponent
{
    public float duration;
    public float targetScale;
    public Action postAction;
}
