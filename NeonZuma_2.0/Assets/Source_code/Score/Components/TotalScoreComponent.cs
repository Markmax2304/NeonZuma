﻿using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Manage, Unique]
public class TotalScoreComponent : IComponent
{
    public int value;
}