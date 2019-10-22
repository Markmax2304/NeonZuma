using UnityEngine;
using Entitas;

/// <summary>
/// Логика определения RayCast коллизий
/// </summary>
public class BallRayCastSystem : IExecuteSystem, IInitializeSystem
{
    private Contexts _contexts;
    private float ballDiametr;
    private LayerMask mask;
    private RaycastHit2D[] hits;

    public BallRayCastSystem(Contexts contexts)
    {
        _contexts = contexts;
        mask = LayerMask.GetMask("Balls");
        hits = new RaycastHit2D[4];
    }

    public void Initialize()
    {
        ballDiametr = _contexts.global.levelConfig.value.ballDiametr;
    }

    public void Execute()
    {
        var balls = _contexts.game.GetEntities(GameMatcher.RayCast);

        foreach(var ball in balls)
        {
            Vector3 position = ball.transform.value.position;
            Vector3 lastPosition = ball.rayCast.lastPosition;

            if(position == lastPosition)
                continue;

            Vector3 direction = (position - lastPosition).normalized;

            int countHits = Physics2D.CircleCastNonAlloc(position, ballDiametr / 2f, direction, hits, ballDiametr / 4f, mask);
            // first hit are always belong to own collider. We are not interested by it
            if (countHits > 1)
            {
                ProcessRayCastCollision(hits, countHits, ball);
            }

            ball.ReplaceRayCast(position);
        }
    }

    #region Private Methods
    private void ProcessRayCastCollision(RaycastHit2D[] hits, int count, GameEntity projectile)
    {
        for(int i = 0; i < count; i++)
        {
            var hitEntity = hits[i].transform.gameObject.GetEntityLink()?.entity;
            if (hitEntity == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to create collision entity. Hit entity is null", TypeLogMessage.Error, true, GetType());
#endif
                return;
            }

            if (hitEntity == projectile)
                continue;

            // projectile collistion stuff
            if (IsProjectileCollision(projectile, hitEntity))
            {
                var type = projectile.isExplosion ? TypeCollision.Explosion : TypeCollision.Projectile;
                _contexts.input.CreateEntity().AddCollision(type, projectile, hitEntity);

#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    string typeCollision = projectile.isExplosion ? TypeCollision.Explosion.ToString() : TypeCollision.Projectile.ToString();
                    _contexts.manage.CreateEntity()
                        .AddLogMessage(string.Format(" ___ Creating collision with type - {0}, handler - {1}, collider - {2}",
                        typeCollision, projectile.ToString(), hitEntity.ToString()), TypeLogMessage.Trace, false, GetType());
                }
#endif
            }
        }
    }

    private bool IsProjectileCollision(GameEntity projectile, GameEntity collider)
    {
        return projectile.isProjectile && collider.hasBallId;
    }
    #endregion
}
