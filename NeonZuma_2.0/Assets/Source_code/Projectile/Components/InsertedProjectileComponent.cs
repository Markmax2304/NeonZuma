using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

[Game]
public class InsertedProjectileComponent : IComponent
{
    public int trackId;
    public int? frontBallId;
}
