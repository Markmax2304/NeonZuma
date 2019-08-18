using System.Collections.Generic;

using UnityEngine;
using Entitas;
using DG.Tweening;

public class MoveAnimationControlSystem : ReactiveSystem<GameEntity>
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
        foreach(var animatedBall in entities)
        {
            Vector3 target = animatedBall.moveAnimation.target;
            float duration = animatedBall.moveAnimation.duration;
            var postAction = animatedBall.moveAnimation.postAction;
            animatedBall.RemoveMoveAnimation();

            var trans = animatedBall.transform.value;
            var tweens = DOTween.TweensByTarget(trans, false, filledList);

            if(tweens == null)
            {
                animatedBall.AddMoveAnimationInfo(new List<TweenCallback>() { () => postAction() });
                trans.DOMove(target, duration).onComplete += delegate ()
                {
                    animatedBall.isMoveAnimationDone = true;
                };
            }
            else if(tweens.Count == 1)
            {
                Debug.Log("mix animation");
                animatedBall.moveAnimationInfo.completeActions.Add(() => postAction());

                DOTween.Kill(trans);
                trans.DOMove(target, duration).onComplete += delegate ()
                {
                    animatedBall.isMoveAnimationDone = true;
                };
            }
            else
            {
                Debug.Log("Failed to set or update animation. Object has more than one tween animations");
                continue;
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
