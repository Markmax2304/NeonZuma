using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class ConnectChainsSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;
    private float offsetBetweenBalls;

    public ConnectChainsSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
        offsetBetweenBalls = _contexts.game.levelConfig.value.offsetBetweenBalls;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach(var coll in entities)
        {
            if(coll.collision.handler == null || coll.collision.collider == null)
            {
                Debug.Log("Failed to connect chain. Collision's entities is null");
                continue;
            }

            GameEntity frontEdge = coll.collision.handler;
            GameEntity backEdge = coll.collision.collider;

            var frontChain = _contexts.game.GetEntitiesWithChainId(backEdge.parentChainId.value).FirstOrDefault();
            var backChain = _contexts.game.GetEntitiesWithChainId(frontEdge.parentChainId.value).FirstOrDefault();
            if(frontChain == null || backChain == null)
            {
                Debug.Log("Failed to conncet chain. front or back chain is null");
                continue;
            }

            float startDistance = frontEdge.distanceBall.value;
            var frontBalls = frontChain.GetChainedBalls(true, true);
            if(frontBalls == null)
            {
                Debug.Log("Failed to conncet chain. front balls is null");
                continue;
            }

            // TODO: add soft animation of shifting
            // TODO QUICKLY: what should happen while inserting and connection is doing in the same time?

            for(int i = 0; i < frontBalls.Count; i++)
            {
                frontBalls[i].ReplaceParentChainId(backChain.chainId.value);
                frontBalls[i].ReplaceDistanceBall(startDistance + offsetBetweenBalls * (i + 1));
            }

            frontChain.Destroy();
            coll.isDestroyed = true;
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasCollision && entity.collision.type == CollisionType.ChainContact;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Collision);
    }
}
