using System.Collections.Generic;
using System.Linq;

using Entitas;
using UnityEngine;
using PathCreation;
using System;

/// <summary>
/// Логика просчёта столкновения снаряда с цепью шаров, а также логика вставки шара в цепь
/// </summary>
public class CollidingAndInsertingProjectileSystem : ReactiveSystem<InputEntity>, IInitializeSystem
{
    private Contexts _contexts;
    private float ballDiametr;
    private float insertDuration;

    public CollidingAndInsertingProjectileSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        ballDiametr = _contexts.global.levelConfig.value.ballDiametr;
        insertDuration = _contexts.global.levelConfig.value.insertDuration;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach (var coll in entities)
        {
            if(coll.collision.collider == null || coll.collision.handler == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to define where in chain should insert ball. Collision's entities is null", 
                    TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            var ball = coll.collision.collider;
            var projectile = coll.collision.handler;

            // some crutch to stop double adding component
            if (projectile.hasParentChainId)
            {
#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($"Trying to insert ball that is inserting already. Projectile: {projectile.ToString()}. Ball: {ball.ToString()}",
                        TypeLogMessage.Trace, false, GetType());
                }
#endif
                continue;
            }

            var chain = _contexts.game.GetEntitiesWithChainId(ball.parentChainId.value).FirstOrDefault();
            if(chain == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to define where in chain should insert ball. chain of ball is null", TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if (track == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to inserting ball in chain. Couldn't get track entity", TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            var balls = chain.GetChainedBalls(true);
            if (balls == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to inserting ball in chain. chain balls is null", TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            int frontBallIndex;
            // frontBall - projectile must be inserted behind this ball, 
            // if projectile must be inserted at first position, frontBall is null
            if (CalculateFrontBallForProjectile(projectile, chain, balls, track, out frontBallIndex))
            {
#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Insert projectile behind ball index: {frontBallIndex.ToString()}", TypeLogMessage.Trace, false, GetType());
                }
#endif
                InsertBall(projectile, frontBallIndex, chain, balls, track);

#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Inserted projectile to chain: projectile {projectile.ToString()} in chain {chain.ToString()}",
                        TypeLogMessage.Trace, false, GetType());
                }
#endif
                // combo
                _contexts.manage.ReplaceMoveBackCombo(0);
            }
            else
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to define where in chain should insert ball. Couldn't find fron ball", TypeLogMessage.Error, true, GetType());
#endif
            }

            coll.isDestroyed = true;
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.collision.type == TypeCollision.Projectile;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Collision);
    }

    #region Private Methods
    private void InsertBall(GameEntity projectile, int frontBallIndex, GameEntity chain, List<GameEntity> chainBalls, GameEntity track)
    {
        chain.ReplaceChainSpeed(0f);
        track.isResetChainEdges = true;
#if UNITY_EDITOR
        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ Start to insert projectile. Mark to reset chain edge tags. And reduce chain speed to zero", 
                TypeLogMessage.Trace, false, GetType());
        }
#endif
        void postChainAction()
        {
            track.isUpdateSpeed = true;
#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage(" ___ Ending animation inserting. Mark track for updating speed", TypeLogMessage.Trace, false, GetType());
            }
#endif
        }

        // first ball
        if (frontBallIndex == -1)
        {
            float distance = chainBalls.First().distanceBall.value + ballDiametr;
            ConvertProjectileToBall(projectile, chain.chainId.value, distance, track.pathCreator.value, postChainAction);
        }
        else
        {
            // last ball
            if (frontBallIndex == chainBalls.Count - 1)
            {
                float distance = chainBalls[frontBallIndex].distanceBall.value - ballDiametr;
                ConvertProjectileToBall(projectile, chain.chainId.value, distance, track.pathCreator.value, postChainAction);
            }
            // another way
            else
            {
                float distance = chainBalls[frontBallIndex].distanceBall.value;

#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Shift balls that's located front of inserted ball. Count of them: {(frontBallIndex + 1).ToString()}",
                        TypeLogMessage.Trace, false, GetType());
                }
#endif
                for (int i = 0; i <= frontBallIndex; i++)
                {
                    float newDistance = chainBalls[i].distanceBall.value + ballDiametr;
                    AnimateShiftBall(chainBalls[i], newDistance, track.pathCreator.value);
                }

                ConvertProjectileToBall(projectile, chain.chainId.value, distance, track.pathCreator.value, postChainAction);
            }
        }
    }

    private bool CalculateFrontBallForProjectile(GameEntity projectile, GameEntity chain, List<GameEntity> chainBalls, GameEntity track, out int frontBallIndex)
    {
        frontBallIndex = -1;
        var pathCreator = track.pathCreator.value;
        float dist = pathCreator.path.GetClosestDistanceAlongPath(projectile.transform.value.position);

        if (chainBalls[0].distanceBall.value < dist)
            return true;

        for(int i = 1; i < chainBalls.Count; i++)
        {
            if(chainBalls[i - 1].distanceBall.value > dist && chainBalls[i].distanceBall.value < dist)
            {
                frontBallIndex = i - 1;
                return true;
            }
        }

        frontBallIndex = chainBalls.Count - 1;
        return true;
    }

    private void ConvertProjectileToBall(GameEntity entity, int chainId, float distanceBall, PathCreator pathCreator, Action postChainAction)
    {
#if UNITY_EDITOR
        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ Convert projectile to ball. projectile: {entity.ToString()}", TypeLogMessage.Trace, false, GetType());
        }
#endif
        entity.isProjectile = false;
        entity.RemoveForce();
        entity.RemoveRayCast();
        entity.transform.value.tag = Constants.BALL_TAG;

        entity.AddDistanceBall(distanceBall);
        entity.AddBallId(Extensions.BallId);
        entity.AddParentChainId(chainId);
        entity.isAddedBall = true;

        Vector3 target = pathCreator.path.GetPointAtDistance(distanceBall, EndOfPathInstruction.Stop);
        postChainAction += delegate ()
        {
            entity.isCheckTargetBall = true;
            _contexts.manage.shootInRowCombo.isProjectile = true;

#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Ending animation inserting. Mark ball as inserted ball: {entity.ToString()}", 
                    TypeLogMessage.Trace, false, GetType());
            }
#endif
        };
        entity.AddMoveAnimation(insertDuration, target, postChainAction);
    }

    private void AnimateShiftBall(GameEntity ball, float distance, PathCreator pathCreator)
    {
        ball.ReplaceDistanceBall(distance);

        Vector3 pos = pathCreator.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop);
        ball.AddMoveAnimation(insertDuration, pos, delegate() { });
    }
    #endregion
}
