using UnityEngine;
using Entitas;

/// <summary>
/// Логика которая клокает счётчик
/// </summary>
public class TickCountersSystem : IExecuteSystem, ITearDownSystem
{
    private Contexts _contexts;

    public TickCountersSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        var counters = _contexts.game.GetEntities(GameMatcher.Counter);
        var deltaTime = _contexts.global.deltaTime.value;

        foreach(var counterEntity in counters)
        {
            float newCount = counterEntity.counter.value - deltaTime;

            if(newCount <= 0)
            {
                counterEntity.counter.postAction();
                counterEntity.RemoveCounter();

#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Finished counter of - {counterEntity.ToString()}", TypeLogMessage.Trace, false, GetType());
                }
#endif
            }
            else
            {
                counterEntity.ReplaceCounter(newCount, counterEntity.counter.postAction);
            }
        }
    }

    public void TearDown()
    {
        var counters = _contexts.game.GetEntities(GameMatcher.Counter);
        foreach(var counter in counters)
        {
            counter.Destroy();
        }
    }
}
