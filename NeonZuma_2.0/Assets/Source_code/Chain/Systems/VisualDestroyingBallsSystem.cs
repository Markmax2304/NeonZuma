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

        foreach(var balls in destroyGroups.Values)
        {
            var chain = _contexts.game.GetEntitiesWithChainId(balls.First().parentChainId.value).SingleEntity();
            float oldChainSpeed = chain.chainSpeed.value;
            chain.ReplaceChainSpeed(0f);

            for(int i = 0; i < balls.Count - 1; i++)
            {
                var ball = balls[i];
                ball.transform.value.DOScale(minScale, destroyDuration).onComplete += delegate ()
                {
                    ball.isDestroyed = true;
                };
            }

            var lastBall = balls.Last();
            lastBall.transform.value.DOScale(minScale, destroyDuration).onComplete += delegate ()
            {
                lastBall.isDestroyed = true;
                chain.isCut = true;
                chain.ReplaceChainSpeed(oldChainSpeed);
            };
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
