using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Флаг о том, что левел начался и будет загружен
/// </summary>
[Manage, Unique, Event(EventTarget.Self)]
public class StartLevelComponent : IComponent
{
}
