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
    private float animDuration;

    private float moveBackSpeed;
    private float moveBackDuration;
    private float increaseMoveBack;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public ConnectChainsSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
        animDuration = _contexts.game.levelConfig.value.alignBallAnimationDuration;
        moveBackSpeed = _contexts.game.levelConfig.value.moveBackSpeed;
        moveBackDuration = _contexts.game.levelConfig.value.moveBackDuration;
        increaseMoveBack = _contexts.game.levelConfig.value.increaseMoveBackFactor;
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
            var pathCreator = track.pathCreator.value;
            float startDistance = frontEdge.distanceBall.value;
            bool isCheckMatch = frontEdge.color.value == backEdge.color.value;

            for (int i = 0; i < frontBalls.Count; i++)
            {
                frontBalls[i].ReplaceParentChainId(backChain.chainId.value);
                float newDistance = startDistance + ballDiametr * (i + 1);

                if (i == 0)
                    AnimateShiftBall(frontBalls[i], newDistance, postConnectAction, pathCreator);
                else
                    AnimateShiftBall(frontBalls[i], newDistance, delegate () { }, pathCreator);

                logger.Trace($" ___ Transfer ball to other chain: {frontBalls[i].ToString()}");
            }

            track.isResetChainEdges = true;
            logger.Trace($" ___ Update track: {track.ToString()}");
            logger.Trace($" ___ And destroy chain: {frontChain.ToString()}");
            frontChain.Destroy();
            coll.isDestroyed = true;

            void postConnectAction()
            {
                if (isCheckMatch)
                {
                    if (frontEdge != null)
                        frontEdge.isCheckTargetBall = true;
                    // maybe it's excess
                    if (backEdge != null)
                        backEdge.isCheckTargetBall = true;

                    logger.Trace($" ___ Mark both ball as ready to check: {frontEdge.ToString()} and {backEdge.ToString()}");

                    // combo
                    int combo = _contexts.game.moveBackCombo.value;
                    _contexts.game.ReplaceMoveBackCombo(combo + 1);

                    // move back
                    backChain.ReplaceChainSpeed(-moveBackSpeed * (1 + increaseMoveBack * combo));
                    backChain.AddCounter(moveBackDuration, delegate ()
                    {
                        track.isUpdateSpeed = true;
                        logger.Trace($" ___ Mark for updating speed after connecting chains");
                    });

                    logger.Trace($" ___ Set move back parameters for chain: {backChain.ToString()}");
                }
                else
                {
                    _contexts.game.ReplaceMoveBackCombo(0);
                    track.isUpdateSpeed = true;
                    logger.Trace($" ___ Mark for updating speed after connecting chains");
                }
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
        ball.AddMoveAnimation(animDuration, pos, action);
    }
    #endregion
}
