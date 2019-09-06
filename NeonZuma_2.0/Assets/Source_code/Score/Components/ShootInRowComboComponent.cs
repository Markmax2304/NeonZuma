using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Manage, Unique]
public class ShootInRowComboComponent : IComponent
{
    public int value;
    public bool isProjectile;
}
