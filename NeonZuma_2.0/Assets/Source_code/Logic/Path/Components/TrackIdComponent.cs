using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Идентификатор трека для упрощённого поиска и связки с цепями
/// </summary>
[Game]
public class TrackIdComponent : IComponent
{
    [EntityIndex]
    public int value;
}
