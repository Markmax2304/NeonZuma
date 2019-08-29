using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public class MoveBackComboComponent : IComponent
{
    public int value;
}
