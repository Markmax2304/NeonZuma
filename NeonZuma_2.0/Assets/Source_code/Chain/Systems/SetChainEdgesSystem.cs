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
                    SetEdgesProperty(balls[0], true, true, true);
                }
                else
                {
                    for (int x = 1; x < balls.Count - 1; x++)
                    {
                        SetEdgesProperty(balls[x], false, false, false);
                    }

                    SetEdgesProperty(balls.First(), true, false, true);
                    SetEdgesProperty(balls.Last(), false, true, false);
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
    private void SetEdgesProperty(GameEntity ball, bool front, bool back, bool overlap)
    {
        bool isChange = ball.isOverlap != overlap;

        ball.isFrontEdge = front;
        ball.isBackEdge = back;
        ball.isOverlap = overlap;

        if (isChange)
        {
            if (overlap)
                logger.Trace($" ___ Add Overlap component - {ball.ToString()}");
            else
                logger.Trace($" ___ Remove Overlap component - {ball.ToString()}");
        }
    }
    #endregion
}
