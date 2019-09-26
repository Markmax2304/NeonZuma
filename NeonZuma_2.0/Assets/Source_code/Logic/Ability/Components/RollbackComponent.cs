using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Флаг отката, который запускает процесс отката
/// </summary>
[Global, Unique]
public class RollbackComponent : IComponent
{
}
