using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Флаг о том, что левел завершён и будет выгружен
/// </summary>
[Manage, Unique, Event(EventTarget.Self)]
public class FinishLevelComponent : IComponent
{
}

