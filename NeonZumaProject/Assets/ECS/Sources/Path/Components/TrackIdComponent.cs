﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class TrackIdComponent : IComponent
{
    [EntityIndex]
    public int value;
}
