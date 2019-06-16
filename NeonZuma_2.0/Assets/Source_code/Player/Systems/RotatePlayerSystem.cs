using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class RotatePlayerSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;

    public RotatePlayerSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        for(int i = 0; i < entities.Count; i++) {
            var players = _contexts.game.GetEntities(GameMatcher.Player);

            foreach(var player in players) {
                if (player.hasTransform) {

                    Transform playerTransform = player.transform.value;
                    Vector2 direction = entities[i].touchPosition.value - (Vector2)playerTransform.position;
                    direction.Normalize();

                    if(entities[i].touchType.value == TypeTouch.Rotate) {
                        playerTransform.up = Vector2.Lerp(playerTransform.up, direction, Time.deltaTime * _contexts.game.levelConfig.value.rotateSpeed);
                    }
                    else if(entities[i].touchType.value == TypeTouch.Shoot) {
                        playerTransform.up = direction;
                    }
                }
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
