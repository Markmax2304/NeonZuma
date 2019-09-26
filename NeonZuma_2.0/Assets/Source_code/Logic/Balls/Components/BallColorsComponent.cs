using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Количество шаров каждого цвета, что сейчас существуют на уровне
/// Ипользуется для рандомизатора цвета шара у жабы
/// </summary>
[Global, Unique]
public class BallColorsComponent : IComponent
{
    public Dictionary<ColorBall, int> value;
}
