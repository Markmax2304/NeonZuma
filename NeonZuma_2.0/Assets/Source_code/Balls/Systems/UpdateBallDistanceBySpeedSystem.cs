using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class UpdateBallDistanceBySpeedSystem : IExecuteSystem
{
    private Contexts _contexts;

    public UpdateBallDistanceBySpeedSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        float delta = _contexts.game.deltaTime.value;

        var paths = _contexts.game.GetEntities(GameMatcher.TrackId);

        foreach(var path in paths)
        {
            var chains = path.GetChains();

            foreach(var chain in chains)
            {
                float speed = chain.chainSpeed.value;
                if (speed == 0)
                    continue;

                var balls = chain.GetChainedBalls();
                int count = balls.Count;

                for (int i = 0; i < count; i++)
                {
                    float distance = balls[i].distanceBall.value;
                    balls[i].ReplaceDistanceBall(distance + delta * speed);
                }
            }
        }
    }
}
