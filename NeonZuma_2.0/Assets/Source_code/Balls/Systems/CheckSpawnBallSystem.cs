using System.Linq;
using UnityEngine;
using Entitas;

public class CheckSpawnBallSystem : IExecuteSystem
{
    private Contexts _contexts;
    private float offsetBetweenBalls;

    public CheckSpawnBallSystem(Contexts contexts)
    {
        _contexts = contexts;
        offsetBetweenBalls = _contexts.game.levelConfig.value.offsetBetweenBalls;
    }

    public void Execute()
    {
        GameEntity[] balls = _contexts.game.GetEntities(GameMatcher.LastBall);

        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i].distanceBall.value >= offsetBetweenBalls)
            {
                var chain = _contexts.game.GetEntitiesWithChainId(balls[i].parentChainId.value).First();
                var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).First();

                track.isTimeToSpawn = true;
            }
        }
    }
}
