using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Global, Unique]
public class ForceSpeedComponent : IComponent
{
    public float value;
}
