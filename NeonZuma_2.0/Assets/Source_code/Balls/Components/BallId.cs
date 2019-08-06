using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class BallId : IComponent
{
    [EntityIndex]
    public int value;               // TODO: excess?
}
