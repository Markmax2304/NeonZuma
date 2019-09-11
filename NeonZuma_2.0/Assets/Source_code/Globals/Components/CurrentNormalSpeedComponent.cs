using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Global, Unique]
public class CurrentNormalSpeedComponent : IComponent
{
    public float value;
}
