using System.Linq;
using System.Collections.Generic;

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
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to connect chain. Collision's entities is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Connect two chains in one. Front and back edges: {frontEdge.ToString()} and {backEdge.ToString()}",
                    TypeLogMessage.Trace, false, GetType());
            }

            var frontChain = _contexts.game.GetEntitiesWithChainId(backEdge.parentChainId.value).FirstOrDefault();
            var backChain = _contexts.game.GetEntitiesWithChainId(frontEdge.parentChainId.value).FirstOrDefault();
            if(frontChain == null || backChain == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to conncet chain. front or back chain is null", TypeLogMessage.Error, true, GetType());
            }

            var track = _contexts.game.GetEntitiesWithTrackId(backChain.parentTrackId.value).FirstOrDefault();
            if(track == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to connect chain. track of chains is null", TypeLogMessage.Error, true, GetType());
            }

            var frontBalls = frontChain.GetChainedBalls(true, true);
            if(frontBalls == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to conncet chain. front balls is null", TypeLogMessage.Error, true, GetType());
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

                if (_contexts.manage.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Transfer ball to other chain: {frontBalls[i].ToString()}", TypeLogMessage.Trace, false, GetType());
                }
            }

            track.isResetChainEdges = true;

            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity().AddLogMessage($" ___ Update track: {track.ToString()}", TypeLogMessage.Trace, false, GetType());
                _contexts.manage.CreateEntity().AddLogMessage($" ___ And destroy chain: {frontChain.ToString()}", 
                    TypeLogMessage.Trace, false, GetType());
            }

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

                    if (_contexts.manage.isDebugAccess)
                    {
                        _contexts.manage.CreateEntity()
                            .AddLogMessage($" ___ Mark both ball as ready to check: {frontEdge.ToString()} and {backEdge.ToString()}", 
                            TypeLogMessage.Trace, false, GetType());
                    }

                    // combo
                    int combo = _contexts.game.moveBackCombo.value;
                    _contexts.game.ReplaceMoveBackCombo(combo + 1);

                    // move back
                    backChain.ReplaceChainSpeed(-moveBackSpeed * (1 + increaseMoveBack * combo));
                    backChain.AddCounter(moveBackDuration, delegate ()
                    {
                        track.isUpdateSpeed = true;
                        if (_contexts.manage.isDebugAccess)
                        {
                            _contexts.manage.CreateEntity()
                                .AddLogMessage($" ___ Mark for updating speed after connecting chains", TypeLogMessage.Trace, false, GetType());
                        }
                    });

                    if (_contexts.manage.isDebugAccess)
                    {
                        _contexts.manage.CreateEntity()
                            .AddLogMessage($" ___ Set move back parameters for chain: {backChain.ToString()}", 
                            TypeLogMessage.Trace, false, GetType());
                    }
                }
                else
                {
                    _contexts.game.ReplaceMoveBackCombo(0);
                    track.isUpdateSpeed = true;
                    if (_contexts.manage.isDebugAccess)
                    {
                        _contexts.manage.CreateEntity().AddLogMessage($" ___ Mark for updating speed after connecting chains", 
                            TypeLogMessage.Trace, false, GetType());
                    }
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
