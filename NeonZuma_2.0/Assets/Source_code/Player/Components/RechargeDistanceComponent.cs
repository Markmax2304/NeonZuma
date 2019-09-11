using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Global, Unique]
public class RechargeDistanceComponent : IComponent
{
    public Vector3 value;
}
