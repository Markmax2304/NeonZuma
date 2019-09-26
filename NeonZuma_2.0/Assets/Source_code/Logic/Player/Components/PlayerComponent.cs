using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Флаг о том, что данная сущность является игроком
/// </summary>
[Game, Unique]
public class PlayerComponent : IComponent
{
}
