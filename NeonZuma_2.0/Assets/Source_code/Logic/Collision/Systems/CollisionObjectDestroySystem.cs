using System.Collections.Generic;
using Entitas;
using UnityEngine;

/// <summary>
/// Логика уничтожения или отключения шаров, если они выходят за пределы игрового экрана
/// </summary>
public class CollisionObjectDestroySystem : ReactiveSystem<InputEntity>, ITearDownSystem
{
    private Contexts _contexts;

    public CollisionObjectDestroySystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach(var coll in entities)
        {
            if (coll.collision.handler == null || coll.collision.collider == null)
            {
                Debug.Log("Failed to proccess with moving out screen. Collision's entities is null");
                continue;
            }

            var gameEntity = coll.collision.collider;

            // if projectile - destroy it
            if (gameEntity.isProjectile)
            {
                gameEntity.DestroyBall();
            }
            // if chain ball - decrement it in color records
            else if (gameEntity.hasDistanceBall)
            {
                gameEntity.isRemovedBall = true;
            }

            coll.isDestroyed = true;
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.collision.type == TypeCollision.OutBorder;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Collision);
    }

    public void TearDown()
    {
        var collisions = _contexts.input.GetEntities(InputMatcher.Collision);
        foreach (var col in collisions)
        {
            col.Destroy();
        }
    }
}
