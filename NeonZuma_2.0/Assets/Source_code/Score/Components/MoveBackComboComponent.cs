﻿using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Manage, Unique]
public class MoveBackComboComponent : IComponent
{
    public int value;
}
