using System.Collections.Generic;

using UnityEngine;
using Entitas;
using DG.Tweening;
using System;

/// <summary>
/// Логика запуска и прерывания анимаций движения объектов
/// </summary>
public class MoveAnimationControlSystem : ReactiveSystem<GameEntity>, ITearDownSystem
{
    private Contexts _contexts;

    private List<Tween> filledList;

    public MoveAnimationControlSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;

        filledList = new List<Tween>();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var animatedEntity in entities)
        {
#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Apply moving animation to object: {animatedEntity.ToString()}", TypeLogMessage.Trace, false, GetType());
            }
#endif
            Vector3 target = animatedEntity.moveAnimation.target;
            float duration = animatedEntity.moveAnimation.duration;
            var postAction = animatedEntity.moveAnimation.postAction;
            animatedEntity.RemoveMoveAnimation();

            var transform = animatedEntity.transform.value;
            var tweens = DOTween.TweensByTarget(transform, false, filledList);

            if (tweens == null || tweens.Count == 0)
            {
                animatedEntity.AddAnimationInfo(new List<Action>() { postAction });

                transform.DOMove(target, duration).onComplete += CompleteAnimationCallback;
            }
            else
            {
                animatedEntity.animationInfo.completeActions.Add(postAction);
                DOTween.Kill(transform);

                transform.DOMove(target, duration).onComplete += CompleteAnimationCallback;
            }

            void CompleteAnimationCallback()
            {
                if (animatedEntity != null)
                {
                    animatedEntity.isAnimationDone = true;

#if UNITY_EDITOR
                    if (_contexts.global.isDebugAccess)
                    {
                        _contexts.manage.CreateEntity()
                            .AddLogMessage($" ___ Added done animation component to: {animatedEntity.ToString()}",
                            TypeLogMessage.Trace, false, GetType());
                    }
#endif
                }
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasMoveAnimation && entity.hasTransform;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.MoveAnimation);
    }

    public void TearDown()
    {
        filledList.Clear();
    }
}
