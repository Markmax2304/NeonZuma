using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Global, Unique]
public class BallColorsComponent : IComponent
{
    public Dictionary<ColorBall, int> value;
}
