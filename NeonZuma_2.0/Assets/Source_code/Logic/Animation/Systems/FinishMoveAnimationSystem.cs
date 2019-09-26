using System.Collections.Generic;

using UnityEngine;
using Entitas;
using DG.Tweening;

/// <summary>
/// Логика обработки окончания анимации и вызова пост событий
/// </summary>
public class FinishMoveAnimationSystem : ReactiveSystem<GameEntity>, ITearDownSystem
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
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Finished animation of object: {animatedBall.ToString()}", TypeLogMessage.Trace, false, this.GetType());
            }
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

    public void TearDown()
    {
        var animations = _contexts.game.GetEntities(GameMatcher.AnimationInfo);
        foreach (var animation in animations)
        {
            if (animation.hasTransform)
            {
                DOTween.Kill(animation.transform.value);
            }

            animation.DestroyBall();
        }
    }
}
