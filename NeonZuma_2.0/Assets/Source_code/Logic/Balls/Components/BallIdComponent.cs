using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Идентификатор шара для упрощённого поиска и идентификации
/// </summary>
[Game]
public class BallIdComponent : IComponent
{
    [EntityIndex]
    public int value;
}
