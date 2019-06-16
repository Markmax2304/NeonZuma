using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class UpdateBallDistanceBySpeedSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    public UpdateBallDistanceBySpeedSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        float delta = _contexts.game.deltaTime.value;
        float speed = _contexts.game.levelConfig.value.followSpeed;

        for(int i = 0; i < entities.Count; i++) {
            float distance = entities[i].distanceBall.value;
            entities[i].ReplaceDistanceBall(distance + delta * speed);
            entities[i].isUpdateDistance = true;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasDistanceBall && entity.hasTransform;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.DistanceBall, GameMatcher.Transform));
    }
}
