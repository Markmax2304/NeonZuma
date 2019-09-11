using System.Collections.Generic;

using Entitas;
using UnityEngine;
using DG.Tweening;

public class BallExchangePlayerSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;

    public BallExchangePlayerSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        if (!_contexts.global.isFireAccess)
            return;

        _contexts.global.isFireAccess = false;

        var shootEntity = _contexts.game.shootEntity;
        var rechargeEntity = _contexts.game.rechargeEntity;

        Transform rechargeParent = rechargeEntity.transform.value.parent;
        Transform shootParent = shootEntity.transform.value.parent;

        Vector3 distance = _contexts.global.rechargeDistance.value;
        float duration = _contexts.global.levelConfig.value.rechargeTime;
        shootEntity.transform.value.DOLocalMove(-distance, duration).onComplete += delegate() 
        {
            ConvertToRecharge(shootEntity, rechargeParent);
        };
        rechargeEntity.transform.value.DOLocalMove(distance, duration).onComplete += delegate () 
        {
            _contexts.global.isFireAccess = true;
            ConvertToShoot(rechargeEntity, shootParent);
        };
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.touchType.value == TypeTouch.Exchange;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.AllOf(InputMatcher.TouchType));
    }

    #region Private Methods
    private void ConvertToShoot(GameEntity entity, Transform newParent)
    {
        entity.transform.value.parent = newParent;
        entity.isShoot = true;
        entity.isRecharge = false;
    }

    private void ConvertToRecharge(GameEntity entity, Transform newParent)
    {
        entity.transform.value.parent = newParent;
        entity.isShoot = false;
        entity.isRecharge = true;
    }
    #endregion
}
