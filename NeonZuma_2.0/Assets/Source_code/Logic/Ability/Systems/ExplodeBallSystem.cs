using System.Collections.Generic;

using UnityEngine;
using Entitas;

/// <summary>
/// Логика взрыва взрывного шара, после его столкновения с другими шарами в цепи
/// Уничтожаются все шары в заданом радиусе
/// </summary>
public class ExplodeBallSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;
    private Collider2D[] hits;
    private int mask;

    public ExplodeBallSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
        hits = new Collider2D[30];
        mask = LayerMask.GetMask("Balls");
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach(var collisionEntity in entities)
        {
            var explosionEntity = collisionEntity.collision.handler;

            if(explosionEntity == null || !explosionEntity.hasTransform)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Failed to get explosion entity: {explosionEntity?.ToString()}", TypeLogMessage.Error, true, GetType());
            }

            // TODO: invoke vfx before kill balls 
            DestroyExplosionArea(explosionEntity, _contexts.global.levelConfig.value.explosionRadius);

            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Acting role of explosion projectile: {explosionEntity.ToString()}", TypeLogMessage.Trace, false, GetType());
            }

            DestroyExplosionProjectile(explosionEntity);
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.collision.type == TypeCollision.Explosion;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Collision);
    }

    #region Private Methods
    private void DestroyExplosionArea(GameEntity explosionEntity, float radius)
    {
        int destroyGroup = Extensions.DestroyGroupId;
        int countHits = Physics2D.OverlapCircleNonAlloc(explosionEntity.transform.value.position, radius, hits, mask);

        for(int i = 0; i < countHits; i++)
        {
            var entityBall = hits[i].gameObject.GetEntityLink().entity;
            if(entityBall == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Some overlaped object as entity is null: {entityBall.ToString()}",
                    TypeLogMessage.Error, true, GetType());
                continue;
            }

            if (entityBall.isExplosion)
                continue;

            entityBall.AddGroupDestroy(destroyGroup);
        }

        // TODO: decide about bonus for exploding balls and implement it here
    }

    private void DestroyExplosionProjectile(GameEntity projectile)
    {
        projectile.isProjectile = false;
        projectile.RemoveForce();
        projectile.RemoveRayCast();
        projectile.isExplosion = false;
        // code above just because in future we can add some animation to this projectile
        projectile.DestroyBall();
    }
    #endregion
}
