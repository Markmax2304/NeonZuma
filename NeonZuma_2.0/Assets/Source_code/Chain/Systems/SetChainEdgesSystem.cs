using System.Linq;
using System.Collections.Generic;

using Entitas;
using UnityEngine;

public class SetChainEdgesSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    public SetChainEdgesSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var track in entities)
        {
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Start setting chain edges and RayCast component. For track - {track.ToString()}", 
                    TypeLogMessage.Trace, false, GetType());
            }

            var chains = track.GetChains(true);
            if(chains == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to update chain edges. Chain collection is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            for(int i = 0; i < chains.Count; i++)
            {
                var balls = chains[i]?.GetChainedBalls(true);

                if (balls == null)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage("Failed to update chain edges. Some chain is null", TypeLogMessage.Error, true, GetType());
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

        if (_contexts.global.isDebugAccess && isChange)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage(overlap ? $" ___ Add Overlap component - {ball.ToString()}"
                : $" ___ Remove Overlap component - {ball.ToString()}", TypeLogMessage.Trace, false, GetType());
        }
    }
    #endregion
}
