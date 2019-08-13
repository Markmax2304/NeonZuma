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
            var chains = track.GetChains(true);
            if(chains == null)
            {
                Debug.Log("Failed to update chain edges. Chain collection is null");
                continue;
            }

            for(int i = 0; i < chains.Count; i++)
            {
                var balls = chains[i]?.GetChainedBalls(true);

                if (balls == null)
                {
                    Debug.Log("Failed tot update chain edges. Some chain is null");
                    continue;
                }

                if (balls.Count == 1)
                {
                    balls[0].transform.value.tag = i == chains.Count - 1 ? 
                        Constants.FRONT_EDGE_BALL_TAG : Constants.BACK_EDGE_BALL_TAG;
                }
                else
                {
                    for (int x = 1; x < balls.Count - 1; x++)
                    {
                        balls[x].transform.value.tag = Constants.BALL_TAG;
                    }

                    balls.First().transform.value.tag = Constants.FRONT_EDGE_BALL_TAG;
                    balls.Last().transform.value.tag = Constants.BACK_EDGE_BALL_TAG;
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
}
