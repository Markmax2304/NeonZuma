using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class FinishMoveAnimationSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    public FinishMoveAnimationSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var animatedBall in entities)
        {
            animatedBall.isMoveAnimationDone = false;

            var animationActions = animatedBall.moveAnimationInfo.completeActions;
            animatedBall.RemoveMoveAnimationInfo();

            foreach(var action in animationActions)
            {
                if (action != null)
                    action();
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isMoveAnimationDone;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.MoveAnimationDone);
    }
}
