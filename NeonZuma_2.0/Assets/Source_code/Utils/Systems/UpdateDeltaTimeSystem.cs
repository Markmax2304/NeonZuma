using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class UpdateDeltaTimeSystem : IExecuteSystem
{
    private Contexts _contexts;

    public UpdateDeltaTimeSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        _contexts.game.ReplaceDeltaTime(Time.deltaTime);
    }
}
