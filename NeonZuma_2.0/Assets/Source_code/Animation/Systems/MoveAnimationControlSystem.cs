using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;
using DG.Tweening;
using System;

public class MoveAnimationControlSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    private List<Tween> filledList;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public MoveAnimationControlSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;

        filledList = new List<Tween>();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var animatedEntity in entities)
        {
            logger.Trace($" ___ Apply moving animation to object: {animatedEntity.ToString()}");

            Vector3 target = animatedEntity.moveAnimation.target;
            float duration = animatedEntity.moveAnimation.duration;
            var postAction = animatedEntity.moveAnimation.postAction;
            animatedEntity.RemoveMoveAnimation();

            var transform = animatedEntity.transform.value;
            var tweens = DOTween.TweensByTarget(transform, false, filledList);

            if (tweens == null || tweens.Count == 0)
            {
                animatedEntity.AddAnimationInfo(new List<Action>() { postAction });
                transform.DOMove(target, duration).onComplete += delegate ()
                {
                    if (animatedEntity != null)
                    {
                        animatedEntity.isAnimationDone = true;
                        logger.Trace($" ___ Added done animation component to: {animatedEntity.ToString()}");
                    }
                };
            }
            else
            {
                animatedEntity.animationInfo.completeActions.Add(postAction);
                DOTween.Kill(transform);

                transform.DOMove(target, duration).onComplete += delegate ()
                {
                    if (animatedEntity != null)
                    {
                        animatedEntity.isAnimationDone = true;
                        logger.Trace($" ___ Added done animation component to: {animatedEntity.ToString()}");
                    }
                };
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
}
