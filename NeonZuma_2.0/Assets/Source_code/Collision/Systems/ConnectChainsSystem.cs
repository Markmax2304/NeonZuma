using System.Linq;
using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;
using PathCreation;
using System;

public class ConnectChainsSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;
    private float ballDiametr;
    private float animSpeed;       // maybe should use less value

    private static Log logger = LogManager.GetCurrentClassLogger();

    public ConnectChainsSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
        animSpeed = _contexts.game.levelConfig.value.alignBallSpeed;
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

            var frontBalls = frontChain.GetChainedBalls(true, true);
            if(frontBalls == null)
            {
                Debug.Log("Failed to conncet chain. front balls is null");
                logger.Error("Failed to conncet chain. front balls is null");
                continue;
            }

            // start to process connecting
            frontChain.ReplaceChainSpeed(0);
            backChain.ReplaceChainSpeed(0);
            var pathCreator = track.pathCreator.value;
            float startDistance = frontEdge.distanceBall.value;

            for (int i = 0; i < frontBalls.Count; i++)
            {
                frontBalls[i].ReplaceParentChainId(backChain.chainId.value);
                float newDistance = startDistance + ballDiametr * (i + 1);

                if (i == 0)
                {
                    AnimateShiftBall(frontBalls[i], newDistance, postConnectAction, pathCreator);
                }
                else
                {
                    AnimateShiftBall(frontBalls[i], newDistance, delegate () { }, pathCreator);
                }

                logger.Trace($" ___ Transfer ball to other chain: {frontBalls[i].ToString()}");
            }

            track.isResetChainEdges = true;
            logger.Trace($" ___ Update track: {track.ToString()}");
            logger.Trace($" ___ And destroy chain: {frontChain.ToString()}");
            frontChain.Destroy();
            coll.isDestroyed = true;

            void postConnectAction()
            {
                track.isUpdateSpeed = true;
                // TODO: add mark for checking balls matching
                logger.Trace($" ___ Mark for updating speed after connecting chains");
            }
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

    #region Private Methods
    private void AnimateShiftBall(GameEntity ball, float newDistance, Action action, PathCreator pathCreator)
    {
        ball.ReplaceDistanceBall(newDistance);

        Vector3 pos = pathCreator.path.GetPointAtDistance(newDistance, EndOfPathInstruction.Stop);
        float distanceToDestination = Vector3.Distance(ball.transform.value.position, pos);
        ball.AddMoveAnimation(distanceToDestination / animSpeed, pos, action);
    }
    #endregion
}
