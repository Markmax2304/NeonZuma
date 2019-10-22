using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public class TrackStorageComponent : IComponent
{
    public TrackStorage value;
}
