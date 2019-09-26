using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Идентификатор родительской цепи для шара, для упрощения поиска
/// </summary>
[Game]
public class ParentChainId : IComponent
{
    [EntityIndex]
    public int value;
}
