using System.Collections.Generic;

using Entitas;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Логика смены текущего снаряда на следующий снаряд. То есть, свап снарядов местами
/// </summary>
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

        // shoot transformation
        shootEntity.AddAnimationInfo(new List<System.Action>() { delegate()
        {
            ConvertToRecharge(shootEntity, rechargeParent);
        } });
        shootEntity.transform.value.DOLocalMove(-distance, duration).onComplete += delegate() 
        {
            if (shootEntity != null)
                shootEntity.isAnimationDone = true;
        };

        // recharge transformation
        rechargeEntity.AddAnimationInfo(new List<System.Action>() { delegate()
        {
            _contexts.global.isFireAccess = true;
            ConvertToShoot(rechargeEntity, shootParent);
        } });
        rechargeEntity.transform.value.DOLocalMove(distance, duration).onComplete += delegate () 
        {
            if (rechargeEntity != null)
                rechargeEntity.isAnimationDone = true;
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
