using System.Collections.Generic;
using Entitas;

public class ShootPlayerSystem : ReactiveSystem<InputEntity>
{
    private Contexts _contexts;

    public ShootPlayerSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            var players = _contexts.game.GetEntities(GameMatcher.Player);

            foreach (var player in players)
            {
                //calculate direction
                //shoot!
            }
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasTouchPosition && entity.touchType.value == TypeTouch.Shoot;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.AllOf(InputMatcher.TouchPosition, InputMatcher.TouchType));
    }
}

