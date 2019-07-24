using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public class BallColorsComponent : IComponent
{
    public Dictionary<ColorBall, int> value;
}
