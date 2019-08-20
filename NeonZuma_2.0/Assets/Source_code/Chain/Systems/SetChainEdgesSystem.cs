using System.Linq;
using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using Entitas;
using UnityEngine;

public class SetChainEdgesSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public SetChainEdgesSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var track in entities)
        {
            logger.Trace($" ___ Start setting chain edges and RayCast component. For track - {track.ToString()}");
            GameController.HasRecordToLog = true;

            var chains = track.GetChains(true);
            if(chains == null)
            {
                Debug.Log("Failed to update chain edges. Chain collection is null");
                logger.Error("Failed to update chain edges. Chain collection is null");
                continue;
            }

            for(int i = 0; i < chains.Count; i++)
            {
                var balls = chains[i]?.GetChainedBalls(true);

                if (balls == null)
                {
                    Debug.Log("Failed to update chain edges. Some chain is null");
                    logger.Error("Failed to update chain edges. Some chain is null");
                    continue;
                }

                if (balls.Count == 1)
                {
                    if(i == chains.Count - 1)
                    {
                        balls[0].transform.value.tag = Constants.FRONT_EDGE_BALL_TAG;
                        UpdateRayCasting(balls[0]);
                    }
                    else
                    {
                        balls[0].transform.value.tag = Constants.BACK_EDGE_BALL_TAG;
                        RemoveRayCasting(balls[0]);
                    }
                }
                else
                {
                    for (int x = 1; x < balls.Count - 1; x++)
                    {
                        balls[x].transform.value.tag = Constants.BALL_TAG;
                        RemoveRayCasting(balls[x]);
                    }

                    var firstBall = balls.First();
                    firstBall.transform.value.tag = Constants.FRONT_EDGE_BALL_TAG;
                    UpdateRayCasting(firstBall);

                    var lastBall = balls.Last();
                    lastBall.transform.value.tag = Constants.BACK_EDGE_BALL_TAG;
                    RemoveRayCasting(lastBall);
                }
            }

            track.isResetChainEdges = false;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isResetChainEdges && entity.hasTrackId;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.ResetChainEdges);
    }

    #region Private Methods
    private void UpdateRayCasting(GameEntity entity)
    {
        if (!entity.hasRayCast)
        {
            entity.AddRayCast(entity.transform.value.position);
            logger.Trace($" ___ Add RayCast component - {entity.ToString()}");
        }
    }

    private void RemoveRayCasting(GameEntity entity)
    {
        if (entity.hasRayCast)
        {
            entity.RemoveRayCast();
            logger.Trace($" ___ Remove RayCast component - {entity.ToString()}");
        }
    }
    #endregion
}
