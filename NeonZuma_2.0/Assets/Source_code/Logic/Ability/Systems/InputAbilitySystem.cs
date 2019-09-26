using UnityEngine;
using Entitas;

/// <summary>
/// Тестовая логика создания флагов абилок по нажатию на нумерные клавиши
/// </summary>
public class InputAbilitySystem : IExecuteSystem
{
    private Contexts _contexts;

    public InputAbilitySystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            _contexts.input.CreateEntity().AddAbilityInput(TypeAbility.Freeze);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            _contexts.input.CreateEntity().AddAbilityInput(TypeAbility.Rollback);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            _contexts.input.CreateEntity().AddAbilityInput(TypeAbility.Pointer);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            _contexts.input.CreateEntity().AddAbilityInput(TypeAbility.Explosion);
        }
    }
}
