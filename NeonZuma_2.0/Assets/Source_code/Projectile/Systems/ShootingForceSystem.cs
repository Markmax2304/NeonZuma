using UnityEngine;
using Entitas;

public class ShootingForceSystem : IExecuteSystem, IInitializeSystem
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
}
