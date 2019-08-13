using System.Collections.Generic;
using System.Linq;

using Entitas;
using UnityEngine;

public class CutChainSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private float minOffsetBetweenBalls;

    public CutChainSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        minOffsetBetweenBalls = _contexts.game.levelConfig.value.offsetBetweenBalls;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var chain in entities)
        {
            var balls = chain.GetChainedBalls(true);
            if(balls == null)
            {
                Debug.Log("Failed to cut chain. Chain balls is null");
                continue;
            }

            int firstIndex = 0;

            for(int i = 1; i < balls.Count; i++)
            {
                if(balls[i-1].distanceBall.value - balls[i].distanceBall.value > minOffsetBetweenBalls * 1.1f)
                {
                    var newChain = CreateEmptyChain(chain.parentTrackId.value);

                    for(int x = firstIndex; x < i; x++)
                    {
                        balls[x].ReplaceParentChainId(newChain.chainId.value);
                    }

                    firstIndex = i;
                }
            }

            chain.isCut = false;

            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if(track == null)
            {
                Debug.Log("Failed to cut chain. Track is null");
                continue;
            }

            track.isResetChainEdges = true;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isCut && entity.hasChainId;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Cut);
    }

    #region Private Methods
    private GameEntity CreateEmptyChain(int trackId)
    {
        var newChain = _contexts.game.CreateEntity();
        newChain.AddChainId(Extensions.ChainId);
        newChain.AddParentTrackId(trackId);
        newChain.AddChainSpeed(0f);
        return newChain;
    }
    #endregion
}
