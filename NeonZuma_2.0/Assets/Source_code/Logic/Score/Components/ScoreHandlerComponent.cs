using UnityEngine;
using UnityEngine.UI;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Global, Unique]
public class ScoreHandlerComponent : IComponent
{
    public Text playerScore;
    public Text botScore;
}
