
using UnityEngine;
using Entitas;

/// <summary>
/// Логика выполнения систем, что представляют игровую логику уровня
/// </summary>
public class ExecuteLevelLogicSystem : IExecuteSystem
{
    private Contexts _contexts;

    public ExecuteLevelLogicSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        if (!_contexts.manage.hasLogicSystems || !_contexts.manage.isLevelPlay)
            return;

        var systems = _contexts.manage.logicSystems.value;
        systems.Execute();
        systems.Cleanup();
    }
}
