using System.Collections.Generic;
using Entitas;

public class BallRechargePlayerSystem : ReactiveSystem<InputEntity>, IInitializeSystem
{
    private Contexts _contexts;

    public BallRechargePlayerSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        
    }

    protected override void Execute(List<InputEntity> entities)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            var players = _contexts.game.GetEntities(GameMatcher.Player);

            foreach (var player in players)
            {
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

    #region Private Methods
    
    #endregion
}
