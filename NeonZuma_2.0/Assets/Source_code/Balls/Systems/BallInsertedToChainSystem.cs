using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Entitas;
using PathCreation;
using DG.Tweening;
using System;

public class BallInsertedToChainSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private float insertDuration;

    public BallInsertedToChainSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        insertDuration = _contexts.game.levelConfig.value.insertDuration;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var projectileEntity in entities)
        {
            var chain = projectileEntity.insertedProjectile.chain;
            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).SingleEntity();
            var balls = chain.GetChainedBalls(true);

            float chainSpeed = chain.chainSpeed.value;
            chain.ReplaceChainSpeed(0f);

            bool postChainAction()
            {
                chain.ReplaceChainSpeed(chainSpeed);
                return true;
            }

            // first ball
            if (projectileEntity.insertedProjectile.frontBall == null)
            {
                float distance = balls.First().distanceBall.value + _contexts.game.levelConfig.value.offsetBetweenBalls;
                ConvertProjectileToBall(projectileEntity, chain.chainId.value, distance, track.pathCreator.value, postChainAction);
            }
            else
            {
                var frontBall = projectileEntity.insertedProjectile.frontBall;

                // last ball
                if(frontBall == balls.Last())
                {
                    float distance = frontBall.distanceBall.value - _contexts.game.levelConfig.value.offsetBetweenBalls;
                    ConvertProjectileToBall(projectileEntity, chain.chainId.value, distance, track.pathCreator.value, postChainAction);
                }
                // another way
                else
                {
                    float distance = frontBall.distanceBall.value;
                    for (int i = 0; i < balls.Count; i++)
                    {
                        if (balls[i].distanceBall.value >= frontBall.distanceBall.value)
                        {
                            float newDistance = balls[i].distanceBall.value + _contexts.game.levelConfig.value.offsetBetweenBalls;
                            AnimateShiftBall(balls[i], newDistance, track.pathCreator.value);
                        }

                    }
                    ConvertProjectileToBall(projectileEntity, chain.chainId.value, distance, track.pathCreator.value, postChainAction);
                }
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasInsertedProjectile;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.InsertedProjectile);
    }

    private void ConvertProjectileToBall(GameEntity entity, int chainId, float distanceBall, PathCreator pathCreator, Func<bool> postChainAction)
    {
        entity.isProjectile = false;
        entity.RemoveForce();
        entity.RemoveInsertedProjectile();
        entity.transform.value.tag = Constants.BALL_TAG;

        Vector3 pos = pathCreator.path.GetPointAtDistance(distanceBall, EndOfPathInstruction.Stop);
        entity.transform.value.DOLocalMove(pos, insertDuration).onComplete += delegate ()
        {
            entity.AddDistanceBall(distanceBall);
            entity.AddBallId(Extensions.BallId);
            entity.AddParentChainId(chainId);
            postChainAction();
        };
    }

    private void AnimateShiftBall(GameEntity ball, float distance, PathCreator pathCreator)
    {
        Vector3 pos = pathCreator.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop);
        ball.transform.value.DOLocalMove(pos, insertDuration * 0.9f).onComplete += delegate ()
        {
            ball.ReplaceDistanceBall(distance);
        };
    }
}
