using System.Collections.Generic;

using UnityEngine;
using Entitas;

/// <summary>
/// Логика вхождения шара на игровой экран
/// </summary>
public class EnteringBallsToScreenSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;

    public EnteringBallsToScreenSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach (var entity in entities)
        {
            var gameEntity = entity.collision.collider;
            gameEntity.isAddedBall = true;
            entity.isDestroyed = true;
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.collision.type == TypeCollision.InBorder;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Collision);
    }
}

