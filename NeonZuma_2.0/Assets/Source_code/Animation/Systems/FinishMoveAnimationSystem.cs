using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class FinishMoveAnimationSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public FinishMoveAnimationSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var animatedBall in entities)
        {
            logger.Trace($" ___ Finished animation of object: {animatedBall.ToString()}");

            animatedBall.isAnimationDone = false;

            var animationActions = animatedBall.animationInfo.completeActions;
            animatedBall.RemoveAnimationInfo();

            foreach(var action in animationActions)
            {
                if (action != null)
                    action();
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isAnimationDone && entity.hasAnimationInfo;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AnimationDone);
    }
}
