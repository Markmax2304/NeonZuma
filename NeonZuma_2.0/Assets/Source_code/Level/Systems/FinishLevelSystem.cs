using System.Collections.Generic;

using UnityEngine;
using Entitas;

/// <summary>
/// Логика окончания игрового уровня и его выгрузка, а также вызов всех событий подписаных на это
/// </summary>
public class FinishLevelSystem : ReactiveSystem<ManageEntity>
{
    private Contexts _contexts;

    public FinishLevelSystem(Contexts contexts) : base(contexts.manage)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<ManageEntity> entities)
    {
        entities[0].isDestroyed = true;

        if (_contexts.manage.hasLogicSystems)
        {
            _contexts.manage.logicSystems.value.TearDown();
            _contexts.manage.isLevelPlay = false;
        }

        // I dont like this part
        InvokeEventListeners();
    }

    protected override bool Filter(ManageEntity entity)
    {
        return entity.isFinishLevel;
    }

    protected override ICollector<ManageEntity> GetTrigger(IContext<ManageEntity> context)
    {
        return context.CreateCollector(ManageMatcher.FinishLevel);
    }

    #region Private Methods
    private void InvokeEventListeners()
    {
        var listeners = _contexts.manage.GetEntities(ManageMatcher.FinishLevelListener);
        for(int i = 0; i < listeners.Length; i++)
        {
            foreach (var action in listeners[i].finishLevelListener.value)
            {
                action.OnFinishLevel(listeners[i]);
            }
        }
    }
    #endregion
}
