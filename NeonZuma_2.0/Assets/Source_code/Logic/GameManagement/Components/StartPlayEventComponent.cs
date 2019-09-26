using System;
using System.Collections.Generic;

using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Список событий, которые выполняются на старте игрового уровня
/// Предназначен для спауна кучи шаров в начале уровня
/// </summary>
[Manage, Unique]
public class StartPlayEventComponent : IComponent
{
    public List<Action> value;
}
