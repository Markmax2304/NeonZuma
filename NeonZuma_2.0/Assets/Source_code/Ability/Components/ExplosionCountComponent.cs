using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Global, Unique]
public class ExplosionCountComponent : IComponent
{
    public int value;
}
