using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CollisionObjectDestroySystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;

    public CollisionObjectDestroySystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach(var entity in entities)
        {
            var gameEntity = entity.collision.collider;

            // if projectile - destroy it
            if (gameEntity.isProjectile)
            {
                gameEntity.isDestroyed = true;
            }
            // if chain ball - decrement it in color records
            else if (gameEntity.hasDistanceBall)
            {
                gameEntity.isRemovedBall = true;
            }

            entity.isDestroyed = true;
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.collision.type == CollisionType.OutBorder;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Collision);
    }
}
