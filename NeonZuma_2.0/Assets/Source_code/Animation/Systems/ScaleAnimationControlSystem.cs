using System;
using System.Collections.Generic;

using UnityEngine;
using Entitas;
using DG.Tweening;

public class ScaleAnimationControlSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    private List<Tween> filledList;

    public ScaleAnimationControlSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;

        filledList = new List<Tween>();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var scaledEntity in entities)
        {
            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Apply scaling animation to object: {scaledEntity.ToString()}", TypeLogMessage.Trace, false, GetType());
            }

            float targetScale = scaledEntity.scaleAnimation.targetScale;
            float duration = scaledEntity.scaleAnimation.duration;
            var postAction = scaledEntity.scaleAnimation.postAction;
            scaledEntity.RemoveScaleAnimation();

            var transform = scaledEntity.transform.value;
            var tweens = DOTween.TweensByTarget(transform, false, filledList);

            if (tweens == null || tweens.Count == 0)
            {
                scaledEntity.AddAnimationInfo(new List<Action>() { postAction });

                transform.DOScale(targetScale, duration).onComplete += delegate ()
                {
                    if (scaledEntity != null)
                    {
                        scaledEntity.isAnimationDone = true;

                        if (_contexts.manage.isDebugAccess)
                        {
                            _contexts.manage.CreateEntity()
                                .AddLogMessage($" ___ Added done animation component to: {scaledEntity.ToString()}", 
                                TypeLogMessage.Trace, false, GetType());
                        }
                    }
                };
            }
            else
            {
                scaledEntity.animationInfo.completeActions.Add(postAction);
                DOTween.Kill(transform);

                transform.DOScale(targetScale, duration).onComplete += delegate ()
                {
                    if (scaledEntity != null)
                    {
                        scaledEntity.isAnimationDone = true;

                        if (_contexts.manage.isDebugAccess)
                        {
                            _contexts.manage.CreateEntity()
                                .AddLogMessage($" ___ Added done animation component to: {scaledEntity.ToString()}", 
                                TypeLogMessage.Trace, false, GetType());
                        }
                    }
                };
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasScaleAnimation && entity.hasTransform;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.ScaleAnimation);
    }
}
