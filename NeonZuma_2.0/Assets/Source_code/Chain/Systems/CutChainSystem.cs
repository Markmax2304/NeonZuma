using System.Collections.Generic;

using UnityEngine;
using Entitas;

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
            var chainBalls = chain.GetChainedBalls(true);
            int firstIndex = 0;

            for(int i = 1; i < chainBalls.Count; i++)
            {
                // place to continue
                // шары ещё не удалены, поэтому разрыва либо не будет, либо он будет в непредсказуемом месте
                if(chainBalls[i-1].distanceBall.value - chainBalls[i].distanceBall.value > minOffsetBetweenBalls)
                {
                    var newChain = _contexts.game.CreateEntity();
                    newChain.AddChainId(Extensions.ChainId);
                    newChain.AddParentTrackId(chain.parentTrackId.value);
                    newChain.AddChainSpeed(0f);

                    for(int x = firstIndex; x < i; x++)
                    {
                        chainBalls[x].ReplaceParentChainId(newChain.chainId.value);
                    }

                    firstIndex = i;
                }
            }
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
}
