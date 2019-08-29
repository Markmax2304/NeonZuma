using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class TickCountersSystem : IExecuteSystem
{
    private Contexts _contexts;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public TickCountersSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        var counters = _contexts.game.GetEntities(GameMatcher.Counter);
        var deltaTime = _contexts.game.deltaTime.value;

        foreach(var counterEntity in counters)
        {
            float newCount = counterEntity.counter.value - deltaTime;

            if(newCount <= 0)
            {
                counterEntity.counter.postAction();
                counterEntity.RemoveCounter();
                logger.Trace($" ___ Finished counter of - {counterEntity.ToString()}");
            }
            else
            {
                counterEntity.ReplaceCounter(newCount, counterEntity.counter.postAction);
            }
        }
    }
}
