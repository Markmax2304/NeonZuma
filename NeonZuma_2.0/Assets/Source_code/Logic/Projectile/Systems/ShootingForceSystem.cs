using UnityEngine;
using Entitas;

/// <summary>
/// Логика движения снаряда во время полёта
/// </summary>
public class ShootingForceSystem : IExecuteSystem, IInitializeSystem, ITearDownSystem
{
    private Contexts _contexts;

    public ShootingForceSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        _contexts.global.SetForceSpeed(_contexts.global.levelConfig.value.forceSpeed);
    }

    public void Execute()
    {
        GameEntity[] entities = _contexts.game.GetEntities(GameMatcher.Projectile);

        for (int i = 0; i < entities.Length; i++)
        {
            Transform ball = entities[i].transform.value;
            Vector2 direction = entities[i].force.value;
            ball.transform.position += (Vector3)direction * _contexts.global.deltaTime.value * _contexts.global.forceSpeed.value;
        }
    }

    public void TearDown()
    {
        if (_contexts.global.hasForceSpeed)
            _contexts.global.RemoveForceSpeed();

        var projectiles = _contexts.game.GetEntities(GameMatcher.Projectile);
        foreach (var projectile in projectiles)
        {
            projectile.DestroyBall();
        }
    }
}
