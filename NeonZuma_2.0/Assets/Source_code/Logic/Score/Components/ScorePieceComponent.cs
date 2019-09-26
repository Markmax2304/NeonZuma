using UnityEngine;
using Entitas;

/// <summary>
/// Данные о том, сколько очков нужно прибавить к общему счётчику
/// </summary>
[Manage]
public class ScorePieceComponent : IComponent
{
    public int value;
}
