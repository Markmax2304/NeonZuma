using System.Linq;
using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class ConnectChainsSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;
    private float ballDiametr;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public ConnectChainsSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach(var coll in entities)
        {
            GameEntity frontEdge = coll.collision.handler;
            GameEntity backEdge = coll.collision.collider;

            if (frontEdge == null || backEdge == null)
            {
                Debug.Log("Failed to connect chain. Collision's entities is null");
                logger.Error("Failed to connect chain. Collision's entities is null");
                GameController.HasRecordToLog = true;
                continue;
            }

            logger.Trace($" ___ Connect two chains in one. Front and back edges: {frontEdge.ToString()} and {backEdge.ToString()}");
            GameController.HasRecordToLog = true;

            var frontChain = _contexts.game.GetEntitiesWithChainId(backEdge.parentChainId.value).FirstOrDefault();
            var backChain = _contexts.game.GetEntitiesWithChainId(frontEdge.parentChainId.value).FirstOrDefault();
            if(frontChain == null || backChain == null)
            {
                Debug.Log("Failed to conncet chain. front or back chain is null");
                logger.Error("Failed to conncet chain. front or back chain is null");
                continue;
            }

            var track = _contexts.game.GetEntitiesWithTrackId(backChain.parentTrackId.value).FirstOrDefault();
            if(track == null)
            {
                Debug.Log("Failed to connect chain. track of chains is null");
                logger.Error("Failed to connect chain. track of chains is null");
                continue;
            }

            float startDistance = frontEdge.distanceBall.value;
            var frontBalls = frontChain.GetChainedBalls(true, true);
            if(frontBalls == null)
            {
                Debug.Log("Failed to conncet chain. front balls is null");
                logger.Error("Failed to conncet chain. front balls is null");
                continue;
            }

            // TODO: add soft animation of shifting
            // TODO QUICKLY: what should happen while inserting and connection is doing in the same time?

            for(int i = 0; i < frontBalls.Count; i++)
            {
                frontBalls[i].ReplaceParentChainId(backChain.chainId.value);
                frontBalls[i].ReplaceDistanceBall(startDistance + ballDiametr * (i + 1));
                logger.Trace($" ___ Transfer ball to other chain: {frontBalls[i].ToString()}");
            }

            track.isResetChainEdges = true;
            logger.Trace($" ___ Update track: {track.ToString()}/nAnd destroy chain: {frontChain.ToString()}");
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
