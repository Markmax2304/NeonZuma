using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using PathCreation;

public class ChangeBallPositionOnPathSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    public ChangeBallPositionOnPathSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        for(int i = 0; i < entities.Count; i++) {
            float distance = entities[i].distanceBall.value;
            PathCreator pathCreator = entities[i].pathCreator.value;

            // Move Position
            Vector2 position = pathCreator.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop);
            // TODO: change to MovePosition()
            entities[i].transform.value.position = position;

            // Rotate
            Vector3 direction = pathCreator.path.GetDirectionAtDistance(0, EndOfPathInstruction.Stop);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.down, direction);
            entities[i].transform.value.rotation = rotation;

            entities[i].isUpdateDistance = false;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasDistanceBall && entity.hasTransform && entity.isUpdateDistance;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.DistanceBall, GameMatcher.Transform, GameMatcher.UpdateDistance));
    }
}
