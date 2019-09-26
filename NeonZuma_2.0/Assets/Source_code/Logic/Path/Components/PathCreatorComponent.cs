using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using PathCreation;

/// <summary>
/// Класс хранящий путь и связаную с ним логику
/// </summary>
[Game]
public class PathCreatorComponent : IComponent
{
    public PathCreator value;
}
