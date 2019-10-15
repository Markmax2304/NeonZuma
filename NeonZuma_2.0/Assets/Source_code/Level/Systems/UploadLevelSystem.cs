using System;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

/// <summary>
/// Логика начала игрового уровня и загрузка его, а также вызов всех событий подписаных на это
/// </summary>
public class UploadLevelSystem : ReactiveSystem<ManageEntity>
{
    private Contexts _contexts;

    public UploadLevelSystem(Contexts contexts) : base(contexts.manage)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<ManageEntity> entities)
    {
        var entity = entities.SingleEntity();
        entity.isDestroyed = true;

        if (_contexts.manage.hasLogicSystems)
        {
            InitializeSingletonComponents();
            _contexts.manage.logicSystems.value.Initialize();
            _contexts.manage.isLevelPlay = true;
        }

        // I dont like this part
        InvokeEventListeners();
    }

    protected override bool Filter(ManageEntity entity)
    {
        return entity.isStartLevel;
    }

    protected override ICollector<ManageEntity> GetTrigger(IContext<ManageEntity> context)
    {
        return context.CreateCollector(ManageMatcher.StartLevel);
    }

    #region Private Methods
    private void InitializeSingletonComponents()
    {
        _contexts.global.SetBallColors(new Dictionary<ColorBall, int>());
        // events
        _contexts.manage.SetStartPlayEvent(new List<Action>());
    }

    private void InvokeEventListeners()
    {
        var listeners = _contexts.manage.GetEntities(ManageMatcher.StartLevelListener);
        for(int i = 0; i < listeners.Length; i++)
        {
            foreach (var action in listeners[i].startLevelListener.value)
            {
                action.OnStartLevel(listeners[i]);
            }
        }
    }
    #endregion
}
