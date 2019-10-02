using System.Collections.Generic;
using System.Linq;

using NLog;
using Log = NLog.Logger;

using Entitas;
using DG.Tweening;
using UnityEngine;
using SpriteGlow;

/// <summary>
/// Логика выстрела снаряда и последующей перезарядки, а также создания нового шара для перезарядки
/// </summary>
public class ShootPlayerSystem : ReactiveSystem<InputEntity>, IInitializeSystem, ITearDownSystem
{
    private Contexts _contexts;
    private PoolObjectKeeper pool;

    private Transform player;
    private Transform shootPlace;
    private Transform rechargePlace;

    // TODO: вынести это в конфиг
    private readonly Vector3 initScale = new Vector3(.01f, .01f, 1f);
    private readonly Vector3 normalScale = new Vector3(.4f, .4f, 1f);

    private static Log logger = LogManager.GetCurrentClassLogger();

    public ShootPlayerSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
        pool = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
    }

    public void Initialize()
    {
        // subscribe to start game event
        _contexts.manage.startPlayEvent.value.Add(InitializeShootPlayer);
    }

    public void InitializeShootPlayer()
    {
#if UNITY_EDITOR
        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ Initialize Shoot player system", TypeLogMessage.Trace, false, GetType());
        }
#endif
        player = _contexts.game.playerEntity.transform.value;
        shootPlace = GameObject.Find("Shoot").transform;
        rechargePlace = GameObject.Find("Recharge").transform;

        _contexts.global.ReplaceRechargeDistance(shootPlace.localPosition - rechargePlace.localPosition);

        CreateRechargeEntity();
        Recharge();
    }

    protected override void Execute(List<InputEntity> entities)
    {
        if (!_contexts.global.isFireAccess)
            return;

        GameEntity player = _contexts.game.playerEntity;
        // TODO: maybe change to multiple touch
        InputEntity touch = entities.First();

        if (player.hasTransform)
        {
            Vector2 playerPosition = player.transform.value.position;
            Vector2 touchPosition = touch.touchPosition.value;

            Shoot((touchPosition - playerPosition).normalized);
            _contexts.global.isFireAccess = false;
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

    public void TearDown()
    {
        if (_contexts.manage.hasStartPlayEvent)
            _contexts.manage.RemoveStartPlayEvent();
    }

    #region Private Methods
    private void Shoot(Vector2 direction)
    {
        GameEntity projectile = _contexts.game.shootEntity;
        projectile.transform.value.parent = null;
        projectile.isShoot = false;
        projectile.isProjectile = true;
        projectile.AddForce(direction);
        projectile.AddRayCast(projectile.transform.value.position);

        int countExplosion = _contexts.global.explosionCount.value;
        if(countExplosion > 0)
        {
            projectile.isExplosion = true;
            _contexts.global.ReplaceExplosionCount(countExplosion - 1);
        }
    }

    private void Recharge()
    {
        GameEntity projectile = _contexts.game.rechargeEntity;
        projectile.isRecharge = false;
        projectile.isShoot = true;

        Transform ball = projectile.transform.value;
        float duration = _contexts.global.levelConfig.value.rechargeTime;
        projectile.AddAnimationInfo(new List<System.Action> { delegate()
        {
            ball.parent = shootPlace;
            _contexts.global.isFireAccess = true;
        } });

        // TODO: change animate envelope to more usefull or interesting
        ball.DOLocalMove(_contexts.global.rechargeDistance.value, duration).onComplete += delegate () 
        {
            if (projectile != null)
                projectile.isAnimationDone = true;
        };

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
        projectile.AddSpriteGlowEffect(ball.GetComponent<SpriteGlowEffect>());
        projectile.AddColor(Randomizer.GetSingleColor());

        ball.tag = Constants.PROJECTILE_TAG;
        ball.gameObject.Link(projectile, _contexts.game);

        float duration = _contexts.global.levelConfig.value.rechargeTime;
        ball.DOScale(normalScale, duration * 0.9f);
    }
    #endregion
}

