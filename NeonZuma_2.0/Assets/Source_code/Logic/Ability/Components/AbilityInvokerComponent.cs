using UnityEngine;
using Entitas;

/// <summary>
/// Тип абилки, которую надо вызвать
/// </summary>
[Input]
public class AbilityInputComponent : IComponent
{
    public TypeAbility value;
}
