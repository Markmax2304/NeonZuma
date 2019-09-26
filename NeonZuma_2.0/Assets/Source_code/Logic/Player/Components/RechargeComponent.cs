using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Флаг о том, что данная сущность является следующий снарядом
/// То есть, снарядом для перезарядки
/// </summary>
[Game, Unique]
public class RechargeComponent : IComponent
{
}
