using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class ParentTrackIdComponent : IComponent
{
    [EntityIndex]
    public int value;
}
