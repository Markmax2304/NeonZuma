using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System.Linq;

public class RotatePlayerSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;

    public RotatePlayerSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        var player = _contexts.game.playerEntity;
        // TODO: maybe change to multiple touch
        InputEntity touch = entities.First();

        if (player.hasTransform)
        {
            Transform playerTransform = player.transform.value;
            Vector2 direction = touch.touchPosition.value - (Vector2)playerTransform.position;
            direction.Normalize();

            if (touch.touchType.value == TypeTouch.Rotate)
            {
                float rotateSpeed = _contexts.game.levelConfig.value.rotateSpeed;
                playerTransform.up = Vector2.Lerp(playerTransform.up, direction, _contexts.game.deltaTime.value * rotateSpeed);
            }
            else if (touch.touchType.value == TypeTouch.Shoot)
            {
                playerTransform.up = direction;
            }

        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasTouchPosition && (entity.touchType.value == TypeTouch.Rotate || entity.touchType.value == TypeTouch.Shoot);
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.AllOf(InputMatcher.TouchPosition, InputMatcher.TouchType));
    }
}
