using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class DestroyGameEntityHandleSystem : ReactiveSystem<GameEntity>
{
    public DestroyGameEntityHandleSystem(Contexts contexts) : base(contexts.game)
    {
    }

    protected override void Execute(List<GameEntity> entities)
    {
        for(int i = 0; i < entities.Count; i++) {
            entities[i].Destroy();
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isDestroyed;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Destroyed);
    }
}
