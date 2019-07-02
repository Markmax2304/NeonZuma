using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public class RechargeDistanceComponent : IComponent
{
    public Vector3 value;
}
