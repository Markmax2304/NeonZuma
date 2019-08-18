using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Entitas;
using PathCreation;
using DG.Tweening;

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
            if(projectileEntity.insertedProjectile.chain == null)
            {
                Debug.Log("Failed to inserting ball in chain. Inserting info is invalid. Chain is null");
                continue;
            }

            var chain = projectileEntity.insertedProjectile.chain;
            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if(track == null)
            {
                Debug.Log("Failed to inserting ball in chain. Couldn't get track entity");
                continue;
            }

            var balls = chain.GetChainedBalls(true);
            if (balls == null)
            {
                Debug.Log("Failed to inserting ball in chain. chain balls is null");
                continue;
            }

            float chainSpeed = chain.chainSpeed.value;
            chain.ReplaceChainSpeed(0f);

            track.isResetChainEdges = true;

            void postChainAction()
            {
                track.isUpdateSpeed = true;
            }

            // first ball
            if (projectileEntity.insertedProjectile.frontBall == null)
            {
                float distance = balls.First().distanceBall.value + _contexts.game.levelConfig.value.ballDiametr;
                ConvertProjectileToBall(projectileEntity, chain.chainId.value, distance, track.pathCreator.value, postChainAction);
            }
            else
            {
                var frontBall = projectileEntity.insertedProjectile.frontBall;

                // last ball
                if(frontBall == balls.Last())
                {
                    float distance = frontBall.distanceBall.value - _contexts.game.levelConfig.value.ballDiametr;
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
                            float newDistance = balls[i].distanceBall.value + _contexts.game.levelConfig.value.ballDiametr;
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

    private void ConvertProjectileToBall(GameEntity entity, int chainId, float distanceBall, PathCreator pathCreator, Action postChainAction)
    {
        entity.isProjectile = false;
        entity.RemoveForce();
        entity.RemoveInsertedProjectile();
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
}
