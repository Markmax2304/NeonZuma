using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// <summary>
/// Логика уничтожения InputEntity
/// </summary>
public class DestroyInputEntityHandleSystem : ReactiveSystem<InputEntity>
{
    public DestroyInputEntityHandleSystem(Contexts contexts) : base(contexts.input)
    {
    }

    protected override void Execute(List<InputEntity> entities)
    {
        for(int i = 0; i < entities.Count; i++)
        {
            entities[i].Destroy();
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.isDestroyed;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Destroyed);
    }
}
