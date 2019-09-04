using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class DestroyManageEntityHandleSystem : ReactiveSystem<ManageEntity>
{
    public DestroyManageEntityHandleSystem(Contexts contexts) : base(contexts.manage)
    {
    }

    protected override void Execute(List<ManageEntity> entities)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].Destroy();
        }
    }

    protected override bool Filter(ManageEntity entity)
    {
        return entity.isDestroyed;
    }

    protected override ICollector<ManageEntity> GetTrigger(IContext<ManageEntity> context)
    {
        return context.CreateCollector(ManageMatcher.Destroyed);
    }
}
