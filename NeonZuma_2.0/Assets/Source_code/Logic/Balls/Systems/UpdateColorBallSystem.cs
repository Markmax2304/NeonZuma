using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// <summary>
/// Логика обновления цвета шара на спрайте
/// </summary>
public class UpdateColorBallSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    public UpdateColorBallSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var entity in entities)
        {
            Color color = Randomizer.ConvertToColor(entity.color.value);
            entity.sprite.value.color = color;
            entity.spriteGlowEffect.value.GlowColor = color;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasColor && entity.hasSprite;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Color, GameMatcher.Sprite));
    }
}
