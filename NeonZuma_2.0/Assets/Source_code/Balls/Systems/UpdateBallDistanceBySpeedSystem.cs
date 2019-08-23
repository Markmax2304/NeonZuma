using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class UpdateBallDistanceBySpeedSystem : IExecuteSystem
{
    private Contexts _contexts;

    private static Log logger = LogManager.GetCurrentClassLogger();

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
            if(chains == null)
            {
                Debug.Log("Failed to update distance ball. Chain collection is null");
                logger.Error("Failed to update distance ball. Chain collection is null");
                GameController.HasRecordToLog = true;
                continue;
            }

            foreach(var chain in chains)
            {
                if (chain == null)
                    continue;

                float speed = chain.chainSpeed.value;
                if (speed == 0)
                    continue;

                var balls = chain.GetChainedBalls();
                if (balls == null)
                    continue;

                for (int i = 0; i < balls.Count; i++)
                {
                    if (balls[i].hasBallId)
                    {
                        float distance = balls[i].distanceBall.value;
                        balls[i].ReplaceDistanceBall(distance + delta * speed);
                    }
                }
            }
        }
    }
}
