using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        foreach (var entity in entities)
        {
            var ball = entity.collision.collider;
            var projectile = entity.collision.handler;

            var path = _contexts.game.GetEntitiesWithTrackId(ball.parentChainId.value).SingleEntity();
            path.isSpawnAccess = false;
            int ballId;
            if (CalculateFrontBallForProjectile(projectile, ball, out ballId))
            {
                projectile.AddInsertedProjectile(ball.parentChainId.value, ballId);
                projectile.ReplaceForce(Vector2.zero); // just test
            }
            else
            {
                projectile.AddInsertedProjectile(ball.parentChainId.value, null);
            }

            entity.isDestroyed = true;
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
    private bool CalculateFrontBallForProjectile(GameEntity projectile, GameEntity collideBall, out int ballId)
    {
        Transform collideTrans = collideBall.transform.value;
        Transform projectileTrans = projectile.transform.value;

        var chain = _contexts.game.GetEntitiesWithChainId(collideBall.parentChainId.value).SingleEntity();
        var chainBalls = chain.GetChainedBalls(true);

        ballId = 0;
        return true;
    }

    private GameEntity GetNearestBall(GameEntity entity, bool front)
    {
        var trackBalls = _contexts.game.GetEntitiesWithParentTrackId(entity.parentTrackId.value);
        trackBalls.Remove(entity);
        GameEntity nearBall = trackBalls.SingleEntity();

        foreach(var ball in trackBalls)
        {
            /*float currentDist = (nearBall.transform.value.position - ball.transform.value.position).sqrMagnitude;
            float dist = (entity.transform.value.position - ball.transform.value.position).sqrMagnitude;
            if(currentDist > dist)
            {

                nearBall = ball;
            }*/
        }

        return null;
    }
    #endregion
}
