using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Идентификатор цепи. Предназначен для упрощённого поиска цепи под ID
/// </summary>
[Game]
public class ChainIdComponent : IComponent
{
    [EntityIndex]
    public int value;
}

