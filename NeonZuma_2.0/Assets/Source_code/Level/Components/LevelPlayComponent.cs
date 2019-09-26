using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Флаг того, что игровой уровень проигрывается, если его отключить, то и проигрывание систем уровня приостановится
/// </summary>
[Manage, Unique, Event(EventTarget.Self)]
public class LevelPlayComponent : IComponent
{
}
