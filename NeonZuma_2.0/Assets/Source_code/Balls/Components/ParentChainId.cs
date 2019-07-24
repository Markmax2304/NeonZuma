using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class ParentChainId : IComponent
{
    [EntityIndex]
    public int value;
}
