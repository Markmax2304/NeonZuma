using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Разрешение на запись в лог
/// Берётся из настроек
/// </summary>
[Global, Unique]
public class DebugAccessComponent : IComponent
{
}
