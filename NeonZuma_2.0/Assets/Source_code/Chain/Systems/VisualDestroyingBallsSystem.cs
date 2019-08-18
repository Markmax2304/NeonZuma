using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Entitas;
using DG.Tweening;

public class VisualDestroyingBallsSystem : ReactiveSystem<GameEntity>, ICleanupSystem
{
    private Contexts _contexts;
    private Dictionary<int, List<GameEntity>> destroyGroups;
    private float destroyDuration;
    private float minScale;

    public VisualDestroyingBallsSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        destroyGroups = new Dictionary<int, List<GameEntity>>();
        destroyDuration = _contexts.game.levelConfig.value.destroyAnimationDuration;
        minScale = _contexts.game.levelConfig.value.minScaleSize;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        for(int i = 0; i < entities.Count; i++) 
        {
            int destroyId = entities[i].groupDestroy.value;
            if (!destroyGroups.ContainsKey(destroyId))
            {
                destroyGroups.Add(destroyId, new List<GameEntity>());
            }
            destroyGroups[destroyId].Add(entities[i]);
        }

        foreach (var balls in destroyGroups.Values)
        {
            var chain = _contexts.game.GetEntitiesWithChainId(balls.First().parentChainId.value).FirstOrDefault();
            if(chain == null)
            {
                Debug.Log("Failed to destroying grouped balls. Chain is null");
                continue;
            }

            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if (track == null)
            {
                Debug.Log("Failed to mark for recover chain speed during destroying balls. Track request return null");
                continue;
            }

            float oldChainSpeed = chain.chainSpeed.value;
            chain.ReplaceChainSpeed(0f);

            for (int i = 0; i < balls.Count; i++)
            {
                var ball = balls[i];
                ball.RemoveParentChainId();
                ball.transform.value.tag = Constants.UNTAGGED_TAG;
                ball.DestroyBall();
                //ball.transform.value.DOScale(minScale, destroyDuration).onComplete += delegate ()
                //{
                //    ball.isDestroyed = true;
                //};
            }

            if (chain.GetChainedBalls() == null)
                chain.Destroy();
            else
                chain.isCut = true;

            track.isUpdateSpeed = true;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGroupDestroy && entity.hasParentChainId;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.GroupDestroy);
    }

    public void Cleanup()
    {
        destroyGroups.Clear();
    }
}
