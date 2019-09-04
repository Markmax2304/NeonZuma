﻿using UnityEngine;
using Entitas;

public class TickCountersSystem : IExecuteSystem
{
    private Contexts _contexts;

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

                if (_contexts.manage.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Finished counter of - {counterEntity.ToString()}", TypeLogMessage.Trace, false);
                }
            }
            else
            {
                counterEntity.ReplaceCounter(newCount, counterEntity.counter.postAction);
            }
        }
    }
}
