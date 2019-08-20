using System.Collections.Generic;
using System.Linq;

using NLog;
using Log = NLog.Logger;

using Entitas;
using UnityEngine;
using PathCreation;
using System;

public class CollidingAndInsertingProjectileSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;
    private float ballDiametr;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public CollidingAndInsertingProjectileSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach (var coll in entities)
        {
            if(coll.collision.collider == null || coll.collision.handler == null)
            {
                Debug.Log("Failed to define where in chain should insert ball. Collision's entities is null");
                logger.Error("Failed to define where in chain should insert ball. Collision's entities is null");
                GameController.HasRecordToLog = true;
                continue;
            }

            var ball = coll.collision.collider;
            var projectile = coll.collision.handler;

            // some crutch to stop double adding component
            if (projectile.hasParentChainId)
            {
                logger.Trace($"Trying to insert ball that is inserting already. Projectile: {projectile.ToString()}. Ball: {ball.ToString()}");
                GameController.HasRecordToLog = true;
                continue;
            }

            var chain = _contexts.game.GetEntitiesWithChainId(ball.parentChainId.value).FirstOrDefault();
            if(chain == null)
            {
                Debug.Log($"Failed to define where in chain should insert ball. chain of ball is null");
                logger.Error($"Failed to define where in chain should insert ball. chain of ball is null");
                GameController.HasRecordToLog = true;
                continue;
            }

            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if (track == null)
            {
                Debug.Log("Failed to inserting ball in chain. Couldn't get track entity");
                logger.Error("Failed to inserting ball in chain. Couldn't get track entity");
                GameController.HasRecordToLog = true;
                continue;
            }

            var balls = chain.GetChainedBalls(true);
            if (balls == null)
            {
                Debug.Log("Failed to inserting ball in chain. chain balls is null");
                logger.Error("Failed to inserting ball in chain. chain balls is null");
                GameController.HasRecordToLog = true;
                continue;
            }

            int frontBallIndex;
            // frontBall - projectile must be inserted behind this ball, 
            // if projectile must be inserted at first position, frontBall is null
            if (CalculateFrontBallForProjectile(projectile, chain, balls, track, out frontBallIndex))
            {
                logger.Trace($" ___ Insert projectile behind ball index: {frontBallIndex.ToString()}");
                GameController.HasRecordToLog = true;
                InsertBall(projectile, frontBallIndex, chain, balls, track);
                logger.Trace($" ___ Inserted projectile to chain: projectile {projectile.ToString()} in chain {chain.ToString()}");
            }
            else
            {
                Debug.Log($"Failed to define where in chain should insert ball. Couldn't find fron ball");
                logger.Error($"Failed to define where in chain should insert ball. Couldn't find fron ball");
                GameController.HasRecordToLog = true;
            }

            coll.isDestroyed = true;
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.collision.type == CollisionType.Projectile;
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
        logger.Trace($" ___ Start to insert projectile. Mark to reset chain edge tags. And reduce chain speed to zero");

        void postChainAction()
        {
            track.isUpdateSpeed = true;
            logger.Trace(" ___ Ending animation inserting. Mark track for updating speed");
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

                logger.Trace($" ___ Shift balls that's located front of inserted ball. Count of them: {(frontBallIndex + 1).ToString()}");
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
        logger.Trace($" ___ Convert projectile to ball. projectile: {entity.ToString()}");
        entity.isProjectile = false;
        entity.RemoveForce();
        entity.RemoveRayCast();
        entity.transform.value.tag = Constants.BALL_TAG;

        entity.AddDistanceBall(distanceBall);       // it breaks the animation, need to correct(applied)
        entity.AddBallId(Extensions.BallId);
        entity.AddParentChainId(chainId);

        entity.isInsertedBall = true;
        postChainAction();

        //Vector3 target = pathCreator.path.GetPointAtDistance(distanceBall, EndOfPathInstruction.Stop);
        //postChainAction += () => entity.isInsertedBall = true;
        //entity.AddMoveAnimation(insertDuration, target, postChainAction);

        //var tween = entity.transform.value.DOLocalMove(pos, insertDuration);
        //tween.onComplete = delegate ()
        //{
        //    entity.isInsertedBall = true;
        //    postChainAction();
        //};
    }

    private void AnimateShiftBall(GameEntity ball, float distance, PathCreator pathCreator)
    {
        ball.ReplaceDistanceBall(distance);

        //Vector3 pos = pathCreator.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop);
        //ball.transform.value.DOLocalMove(pos, insertDuration * 0.9f).onComplete += delegate ()
        //{
        //    ball.ReplaceDistanceBall(distance);
        //};
    }
    #endregion
}
