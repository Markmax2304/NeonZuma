using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// <summary>
/// Тип действия, которое будет вызвано тачем
/// </summary>
[Input]
public class TouchTypeComponent : IComponent
{
    public TypeTouch value;
}
