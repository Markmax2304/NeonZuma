using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Entitas;
using DG.Tweening;

public class ShootBotSystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    private Contexts contexts;
    private LevelConfig config;
    private PoolObjectKeeper pool;

    public ShootBotSystem(Contexts contexts) : base(contexts.game)
    {
        this.contexts = contexts;
        config = contexts.global.levelConfig.value;
        pool = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
    }

    public void Initialize()
    {
        contexts.manage.startPlayEvent.value.Add(InitializeShootBot);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var botEntity in entities)
        {
            if (botEntity.hasTransform)
            {
                Vector2 forceDirection = botEntity.transform.value.up;

                ShootProjectile(botEntity, forceDirection);
                ChargeProjectile(botEntity);

                botEntity.isRequiredTask = true;
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isBot 
            && entity.hasProjectileInstance 
            && entity.hasBotState 
            && entity.botState.value == BotStateType.Shoot;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BotState);
    }

    #region Private Methods

    private void InitializeShootBot()
    {
        var botEntities = contexts.game.GetEntities(GameMatcher.Bot);

        foreach (var botEntity in botEntities)
        {
            Transform botTransform = botEntity.transform.value;
            botEntity.AddShootPlace(botTransform.Find("Shoot"));

            ChargeProjectile(botEntity);
        }
    }

    private void ShootProjectile(GameEntity botEntity, Vector2 direction)
    {
        GameEntity projectile = botEntity.projectileInstance.value;
        projectile.transform.value.parent = null;
        projectile.isProjectile = true;
        projectile.AddForce(direction);
        projectile.AddRayCast(projectile.transform.value.position);

        botEntity.RemoveProjectileInstance();
    }

    private void ChargeProjectile(GameEntity botEntity)
    {
        Transform shootPlace = botEntity.shootPlace.value;
        Transform ball = pool.RealeseObject(shootPlace.position, shootPlace.rotation, config.initScale).transform;
        ball.parent = shootPlace;

        GameEntity projectile = contexts.game.CreateEntity();
        projectile.AddTransform(ball);
        projectile.AddSprite(ball.GetComponent<SpriteRenderer>());
        projectile.AddColor(Randomizer.GetSingleColor());

        ball.tag = Constants.PROJECTILE_TAG;
        ball.gameObject.Link(projectile, contexts.game);

        // attach projectile to bot
        botEntity.AddProjectileInstance(projectile);

        float duration = contexts.global.levelConfig.value.rechargeTime;
        ball.DOScale(config.normalScale, duration * 0.9f);
    }

    #endregion Private Methods
}
