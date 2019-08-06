using System.Collections.Generic;
using System.Linq;

using Entitas;
using DG.Tweening;
using UnityEngine;

public class ShootPlayerSystem : ReactiveSystem<InputEntity>, IInitializeSystem
{
    private Contexts _contexts;
    private PoolObjectKeeper pool;

    private Transform player;
    private Transform shootPlace;
    private Transform rechargePlace;

    private readonly Vector3 initScale = new Vector3(.01f, .01f, 1f);
    private readonly Vector3 normalScale = new Vector3(.4f, .4f, 1f);

    public ShootPlayerSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
        pool = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
    }

    public void Initialize()
    {
        player = _contexts.game.playerEntity.transform.value;
        shootPlace = GameObject.Find("Shoot").transform;
        rechargePlace = GameObject.Find("Recharge").transform;

        _contexts.game.ReplaceRechargeDistance(shootPlace.position - rechargePlace.position);

        CreateRechargeEntity();
        Recharge();
    }

    protected override void Execute(List<InputEntity> entities)
    {
        if (!_contexts.game.isFireAccess)
            return;

        GameEntity player = _contexts.game.playerEntity;
        // TODO: maybe change to multiple touch
        InputEntity touch = entities.First();

        if (player.hasTransform)
        {
            Vector2 playerPosition = player.transform.value.position;
            Vector2 touchPosition = touch.touchPosition.value;

            Shoot((touchPosition - playerPosition).normalized);
            _contexts.game.isFireAccess = false;
            Recharge();
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasTouchPosition && entity.touchType.value == TypeTouch.Shoot;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.AllOf(InputMatcher.TouchPosition, InputMatcher.TouchType));
    }

    #region Private Methods
    private void Shoot(Vector2 direction)
    {
        GameEntity projectile = _contexts.game.shootEntity;
        projectile.transform.value.parent = null;
        projectile.isShoot = false;
        projectile.isProjectile = true;
        projectile.AddForce(direction);
    }

    private void Recharge()
    {
        GameEntity projectile = _contexts.game.rechargeEntity;
        projectile.isRecharge = false;
        projectile.isShoot = true;

        Transform ball = projectile.transform.value;
        float duration = _contexts.game.levelConfig.value.rechargeTime;
        // TODO: change animate envelope to more usefull or interesting
        ball.DOLocalMove(_contexts.game.rechargeDistance.value, duration).onComplete += delegate () {
            ball.parent = shootPlace; _contexts.game.isFireAccess = true; };

        CreateRechargeEntity();
    }

    private void CreateRechargeEntity()
    {
        Transform ball = pool.RealeseObject(rechargePlace.position, rechargePlace.rotation, initScale).transform;
        ball.parent = rechargePlace;

        GameEntity projectile = _contexts.game.CreateEntity();
        projectile.isRecharge = true;
        projectile.AddTransform(ball);
        projectile.AddSprite(ball.GetComponent<SpriteRenderer>());
        projectile.AddColor(Randomizer.GetSingleColor());

        ball.tag = Constants.PROJECTILE_TAG;
        ball.gameObject.Link(projectile, _contexts.game);

        float duration = _contexts.game.levelConfig.value.rechargeTime;
        ball.DOScale(normalScale, duration * 0.9f);
    }
    #endregion
}

