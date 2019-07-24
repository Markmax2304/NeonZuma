using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class ChainIdComponent : IComponent
{
    [EntityIndex]
    public int value;
}

