using Entitas;
using Entitas.CodeGeneration.Attributes;

[Global, Unique]
public class DeltaTimeComponent : IComponent
{
    public float value;
}
