using UnityEngine;
using Entitas;

public class ShootingForceSystem : IExecuteSystem
{
    private Contexts _contexts;

    public ShootingForceSystem(Contexts contexts)
    {
        _contexts = contexts;
    }
    
    public void Execute()
    {
        GameEntity[] entities = _contexts.game.GetEntities(GameMatcher.Projectile);

        for (int i = 0; i < entities.Length; i++)
        {
            Transform ball = entities[i].transform.value;
            Vector2 direction = entities[i].force.value;
            ball.transform.position += (Vector3)direction * _contexts.game.deltaTime.value * _contexts.game.levelConfig.value.forceSpeed;
        }
    }
}
