using System.Collections.Generic;
using System.Linq;

using Entitas;

public class ProjectileCollidingWithBallSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;

    public ProjectileCollidingWithBallSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach (var collideEntity in entities)
        {
            var ball = collideEntity.collision.collider;
            var projectile = collideEntity.collision.handler;

            // some crutch to stop double adding component
            if (projectile.hasInsertedProjectile)
                continue;

            var chain = _contexts.game.GetEntitiesWithChainId(ball.parentChainId.value).SingleEntity();

            GameEntity frontBall;
            // ballId - projectile must be inserted behind this ball, 
            // if projectile must be inserted at first position, ballId is null
            CalculateFrontBallForProjectile(projectile, ball, out frontBall);
            projectile.AddInsertedProjectile(chain, frontBall);

            collideEntity.isDestroyed = true;
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
    private void CalculateFrontBallForProjectile(GameEntity projectile, GameEntity collideBall, out GameEntity frontBall)
    {
        frontBall = null;

        var chain = _contexts.game.GetEntitiesWithChainId(collideBall.parentChainId.value).SingleEntity();
        var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).SingleEntity();
        var pathCreator = track.pathCreator.value;

        float dist = pathCreator.path.GetClosestDistanceAlongPath(projectile.transform.value.position);
        var chainBalls = chain.GetChainedBalls(true);

        // MAYBE TODO: sort chain is shown better perfomance, but less easy to use
        //chainBalls.Sort((b1, b2) => GetDistance(projectile, b1).CompareTo(GetDistance(projectile, b2)));

        if(chainBalls[0].distanceBall.value < dist)
            return;

        for(int i = 1; i < chainBalls.Count; i++)
        {
            if(chainBalls[i - 1].distanceBall.value > dist && chainBalls[i].distanceBall.value < dist)
            {
                frontBall = chainBalls[i - 1];
                return;
            }
        }

        frontBall = chainBalls.Last();
    }

    //private float GetDistance(GameEntity ball1, GameEntity ball2)
    //{
    //    return Vector3.Distance(ball1.transform.value.position, ball2.transform.value.position);
    //}
    #endregion
}
