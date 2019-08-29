using System;
using System.Collections.Generic;

using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Manage, Unique]
public class StartPlayEventComponent : IComponent
{
    public List<Action> value;
}
