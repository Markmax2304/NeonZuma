using UnityEngine;
using Entitas;

/// <summary>
/// Логика инициализации игрока
/// </summary>
public class InitializePlayerSystem : IInitializeSystem, ITearDownSystem
{
    private Contexts _contexts;

    public InitializePlayerSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        GameEntity playerEntity = _contexts.game.CreateEntity();
        playerEntity.isPlayer = true;

        // TODO: использовать pool или глобальный компонент для хранения, чтобы не создавать каждый раз заново
        GameObject player = GameObject.Instantiate(_contexts.global.levelConfig.value.playerPrefab);
        playerEntity.AddTransform(player.transform);
        var lineRenderer = player.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        playerEntity.AddLineRenderer(lineRenderer);
    }

    public void TearDown()
    {
        var shoot = _contexts.game.shootEntity;
        if (shoot != null)
        {
            shoot.DestroyBall();
        }

        var recharge = _contexts.game.rechargeEntity;
        if (recharge != null)
        {
            recharge.DestroyBall();
        }

        var playerTransform = _contexts.game.playerEntity?.transform.value;
        if (playerTransform != null)
        {
            GameObject.Destroy(playerTransform.gameObject);
            _contexts.game.playerEntity.Destroy();
        }

        _contexts.global.isFireAccess = false;
        if (_contexts.global.hasRechargeDistance)
        {
            _contexts.global.RemoveRechargeDistance();
        }

        var projectiles = _contexts.game.GetEntities(GameMatcher.Projectile);
        foreach (var projectile in projectiles)
        {
            projectile.DestroyBall();
        }

        if (_contexts.global.hasForceSpeed)
        {
            _contexts.global.RemoveForceSpeed();
        }
    }
}
