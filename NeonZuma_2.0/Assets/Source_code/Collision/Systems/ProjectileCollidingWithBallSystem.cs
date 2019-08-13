using System.Collections.Generic;
using System.Linq;

using Entitas;
using UnityEngine;

public class ProjectileCollidingWithBallSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;

    public ProjectileCollidingWithBallSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach (var coll in entities)
        {
            if(coll.collision.collider == null || coll.collision.handler == null)
            {
                Debug.Log("Failed to define where in chain should insert ball. Collision's entities is null");
                continue;
            }

            var ball = coll.collision.collider;
            var projectile = coll.collision.handler;

            // some crutch to stop double adding component
            if (projectile.hasInsertedProjectile)
            {
                continue;
            }

            var chain = _contexts.game.GetEntitiesWithChainId(ball.parentChainId.value).FirstOrDefault();
            if(chain == null)
            {
                Debug.Log($"Failed to define where in chain should insert ball. chain of ball is null");
                continue;
            }

            GameEntity frontBall;
            // frontBall - projectile must be inserted behind this ball, 
            // if projectile must be inserted at first position, frontBall is null
            if (CalculateFrontBallForProjectile(projectile, chain, out frontBall))
            {
                projectile.AddInsertedProjectile(chain, frontBall);
            }
            else
            {
                Debug.Log($"Failed to define where in chain should insert ball. Couldn't find fron ball");
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
    private bool CalculateFrontBallForProjectile(GameEntity projectile, GameEntity chain, out GameEntity frontBall)
    {
        frontBall = null;

        var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
        if(track == null)
            return false;

        var pathCreator = track.pathCreator.value;

        float dist = pathCreator.path.GetClosestDistanceAlongPath(projectile.transform.value.position);
        var chainBalls = chain.GetChainedBalls(true);
        if (chainBalls == null)
            return false;

        if (chainBalls[0].distanceBall.value < dist)
            return true;

        for(int i = 1; i < chainBalls.Count; i++)
        {
            if(chainBalls[i - 1].distanceBall.value > dist && chainBalls[i].distanceBall.value < dist)
            {
                frontBall = chainBalls[i - 1];
                return true;
            }
        }

        frontBall = chainBalls.Last();
        return true;
    }
    #endregion
}
