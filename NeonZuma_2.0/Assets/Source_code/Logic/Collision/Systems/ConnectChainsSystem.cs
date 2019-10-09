using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Entitas;
using PathCreation;
using System;

/// <summary>
/// Логика соединения цепей шаров после их столкновения
/// </summary>
public class ConnectChainsSystem : ReactiveSystem<InputEntity>, IInitializeSystem
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
    }

    public void Initialize()
    {
        ballDiametr = _contexts.global.levelConfig.value.ballDiametr;
        animDuration = _contexts.global.levelConfig.value.alignBallAnimationDuration;
        moveBackSpeed = _contexts.global.levelConfig.value.moveBackSpeed;
        moveBackDuration = _contexts.global.levelConfig.value.moveBackDuration;
        increaseMoveBack = _contexts.global.levelConfig.value.increaseMoveBackFactor;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach(var coll in entities)
        {
            GameEntity frontEdge = coll.collision.handler;
            GameEntity backEdge = coll.collision.collider;

            if (frontEdge == null || backEdge == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to connect chain. Collision's entities is null", TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Connect two chains in one. Front and back edges: {frontEdge.ToString()} and {backEdge.ToString()}",
                    TypeLogMessage.Trace, false, GetType());
            }
#endif
            var frontChain = _contexts.game.GetEntitiesWithChainId(backEdge.parentChainId.value).FirstOrDefault();
            var backChain = _contexts.game.GetEntitiesWithChainId(frontEdge.parentChainId.value).FirstOrDefault();
            if(frontChain == null || backChain == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to conncet chain. front or back chain is null", TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            var track = _contexts.game.GetEntitiesWithTrackId(backChain.parentTrackId.value).FirstOrDefault();
            if(track == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to connect chain. track of chains is null", TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            var frontBalls = frontChain.GetChainedBalls(true, true);
            if(frontBalls == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to conncet chain. front balls is null", TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            // start to process connecting
            var pathCreator = track.pathCreator.value;
            float startDistance = frontEdge.distanceBall.value;
            bool isCheckMatch = frontEdge.color.value == backEdge.color.value;

            for (int i = 0; i < frontBalls.Count; i++)
            {
                float newDistance = startDistance + ballDiametr * (i + 1);
                frontBalls[i].ReplaceDistanceBall(newDistance);
                frontBalls[i].ReplaceParentChainId(backChain.chainId.value);

#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Transfer ball to other chain: {frontBalls[i].ToString()}", TypeLogMessage.Trace, false, GetType());
                }
#endif
            }

            track.isResetChainEdges = true;

#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity().AddLogMessage($" ___ Update track: {track.ToString()}", TypeLogMessage.Trace, false, GetType());
                _contexts.manage.CreateEntity().AddLogMessage($" ___ And destroy chain: {frontChain.ToString()}", 
                    TypeLogMessage.Trace, false, GetType());
            }
#endif
            frontChain.Destroy();
            coll.isDestroyed = true;
            CheckingAfterChainMerging(isCheckMatch, frontEdge, backEdge, backChain, track);
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasCollision && entity.collision.type == TypeCollision.ChainContact;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Collision);
    }

    #region Private Methods
    private void CheckingAfterChainMerging(bool isCheckMatch, GameEntity frontEdge, GameEntity backEdge, GameEntity backChain, GameEntity track)
    {
        if (isCheckMatch)
        {
            if (frontEdge != null)
                frontEdge.isCheckTargetBall = true;
            if (backEdge != null)
                backEdge.isCheckTargetBall = true;

#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Mark both ball as ready to check: {frontEdge.ToString()} and {backEdge.ToString()}",
                    TypeLogMessage.Trace, false, GetType());
            }
#endif
            // combo
            int combo = _contexts.manage.moveBackCombo.value;
            _contexts.manage.ReplaceMoveBackCombo(combo + 1);

            // move back
            backChain.ReplaceChainSpeed(-moveBackSpeed * (1 + increaseMoveBack * combo));
            backChain.AddCounter(moveBackDuration, delegate ()
            {
                track.isUpdateSpeed = true;
#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Mark for updating speed after connecting chains", TypeLogMessage.Trace, false, GetType());
                }
#endif
            });

#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Set move back parameters for chain: {backChain.ToString()}",
                    TypeLogMessage.Trace, false, GetType());
            }
#endif
        }
        else
        {
            _contexts.manage.ReplaceMoveBackCombo(0);
            track.isUpdateSpeed = true;
#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity().AddLogMessage($" ___ Mark for updating speed after connecting chains",
                    TypeLogMessage.Trace, false, GetType());
            }
#endif
        }
    }

    #endregion
}
