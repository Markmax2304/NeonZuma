using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Представляет кешированую deltaTime
/// На протяжении одного цикла выполнения всех систем, Time.deltaTime может меняться, что приведёт к неверным расчётам
/// </summary>
[Global, Unique]
public class DeltaTimeComponent : IComponent
{
    public float value;
}
