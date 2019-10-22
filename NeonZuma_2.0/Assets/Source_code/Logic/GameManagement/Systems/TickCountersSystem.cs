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

        for(int i = 0; i < counters.Length; i++)
        {
            float newCount = counters[i].counter.value - deltaTime;

            if(newCount <= 0)
            {
                counters[i].counter.postAction();
                counters[i].RemoveCounter();

#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Finished counter of - {counters[i].ToString()}", TypeLogMessage.Trace, false, GetType());
                }
#endif
            }
            else
            {
                counters[i].ReplaceCounter(newCount, counters[i].counter.postAction);
            }
        }
    }

    public void TearDown()
    {
        var counters = _contexts.game.GetEntities(GameMatcher.Counter);
        for (int i = 0; i < counters.Length; i++)
        {
            counters[i].Destroy();
        }
    }
}
