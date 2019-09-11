using System.Collections.Generic;

using UnityEngine;
using Entitas;
using System.Linq;

public class InvokingAbilitySystem : ReactiveSystem<InputEntity>, IInitializeSystem
{
    private Contexts _contexts;

    public InvokingAbilitySystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        _contexts.global.SetExplosionCount(0);
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach(var abilityEntity in entities)
        {
            switch (abilityEntity.abilityInput.value)
            {
                case TypeAbility.Freeze:
                    InvokeFreeze();
                    break;
                case TypeAbility.Rollback:
                    InvokeRollback();
                    break;
                case TypeAbility.Pointer:
                    InvokePointer();
                    break;
                case TypeAbility.Explosion:
                    InvokeExplosion();
                    break;
            }

            abilityEntity.isDestroyed = true;
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasAbilityInput;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.AbilityInput);
    }

    // TODO: отменять одной способностью другую
    #region Ability Methods
    private void InvokeFreeze()
    {
        _contexts.global.isFreeze = true;
        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity().AddLogMessage("Activate freeze ability.", TypeLogMessage.Trace, false, GetType());
        }

        _contexts.game.CreateEntity().AddCounter(_contexts.global.levelConfig.value.freezeDuration, delegate ()
        {
            _contexts.global.isFreeze = false;
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity().AddLogMessage("Deactivate freeze ability.", TypeLogMessage.Trace, false, GetType());
            }
        });

        // TODO: vfx
        Debug.Log("Freeze ability. Do some cool effects");
    }

    private void InvokeRollback()
    {
        _contexts.global.isRollback = true;
        MarkAllTracksForUpdatingSpeed();

        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage("Activate Rollback ability. Mark all track for updating speed", TypeLogMessage.Trace, false, GetType());
        }

        _contexts.game.CreateEntity().AddCounter(_contexts.global.levelConfig.value.rollbackDuration, delegate ()
        {
            _contexts.global.isRollback = false;
            MarkAllTracksForUpdatingSpeed();

            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Deactivate Rollback ability. Mark all track for updating speed", TypeLogMessage.Trace, false, GetType());
            }
        });

        // TODO: vfx
        Debug.Log("Rollback ability. Do some cool effect");
    }

    private void InvokePointer()
    {
        var player = _contexts.game.playerEntity;
        player.lineRenderer.value.enabled = true;
        _contexts.global.isPointer = true;
        _contexts.global.ReplaceForceSpeed(_contexts.global.levelConfig.value.pointerShootSpeed);

        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage("Activate Pointer ability. Enable LineRenderer. Speed up force speed", TypeLogMessage.Trace, false, GetType());
        }

        _contexts.game.CreateEntity().AddCounter(_contexts.global.levelConfig.value.pointerDuration, delegate ()
        {
            player.lineRenderer.value.enabled = false;
            _contexts.global.isPointer = false;
            _contexts.global.ReplaceForceSpeed(_contexts.global.levelConfig.value.forceSpeed);

            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Deactivate Pointer ability. Disable LineRenderer. Slow down force speed", TypeLogMessage.Trace, false, GetType());
            }
        });

        // TODO: vfx
        Debug.Log("Pointer ability, but without cool effect");
    }

    private void InvokeExplosion()
    {
        var shootEntity = _contexts.game.GetEntities(GameMatcher.Shoot).FirstOrDefault();
        if(shootEntity == null)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage("Failed to get shoot entity", TypeLogMessage.Error, true, GetType());
        }

        _contexts.global.ReplaceExplosionCount(_contexts.global.explosionCount.value + 1);

        // TODO: vfx
        Debug.Log("Explosion ability. Do a lot of cool effects");
    }
    #endregion

    #region Private Methods
    private void MarkAllTracksForUpdatingSpeed()
    {
        var tracks = _contexts.game.GetEntities(GameMatcher.TrackId);

        if(tracks == null || tracks.Length == 0)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage("Any track entity doesn't exist.", TypeLogMessage.Error, true, GetType());
        }

        foreach(var track in tracks)
        {
            track.isUpdateSpeed = true;
        }
    }
    #endregion
}
