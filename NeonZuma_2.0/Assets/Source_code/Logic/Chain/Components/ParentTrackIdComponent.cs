using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Идентификатор родительского трека цепи для упрощённой связки цепи с треком
/// </summary>
[Game]
public class ParentTrackIdComponent : IComponent
{
    [EntityIndex]
    public int value;
}
