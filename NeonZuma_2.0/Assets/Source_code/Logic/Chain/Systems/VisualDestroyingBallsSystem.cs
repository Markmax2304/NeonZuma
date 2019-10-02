using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Entitas;
using DG.Tweening;

/// <summary>
/// Визуальное уничтожение шаров по группам
/// </summary>
public class VisualDestroyingBallsSystem : ReactiveSystem<GameEntity>, IInitializeSystem, ICleanupSystem
{
    private Contexts _contexts;
    private Dictionary<int, List<GameEntity>> destroyGroups;
    private float destroyDuration;
    private float minScale;

    public VisualDestroyingBallsSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        destroyGroups = new Dictionary<int, List<GameEntity>>();
    }

    public void Initialize()
    {
        destroyDuration = _contexts.global.levelConfig.value.destroyAnimationDuration;
        minScale = _contexts.global.levelConfig.value.minScaleSize;
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
#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Destroy balls with goupId {balls[0].groupDestroy.value}", TypeLogMessage.Trace, false, GetType());
            }
#endif
            var chain = _contexts.game.GetEntitiesWithChainId(balls.First().parentChainId.value).FirstOrDefault();
            if (chain == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to destroying grouped balls. Chain is null", TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if (track == null)
            {
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to mark for recover chain speed during destroying balls. Track request return null",
                    TypeLogMessage.Error, true, GetType());
#endif
                continue;
            }

            DestroyBalls(balls);
            chain.isCut = true;

#if UNITY_EDITOR
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Mark chain for cutting", TypeLogMessage.Trace, false, GetType());
            }
#endif
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

    #region Private Methods
    private void DestroyBalls(List<GameEntity> balls)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            var ball = balls[i];
            ball.RemoveBallId();
            ball.RemoveParentChainId();
            ball.transform.value.tag = Constants.UNTAGGED_TAG;
            ball.isRemovedBall = true;
            ball.isBackEdge = false;        // just for stable
            ball.isFrontEdge = false;       // just for stable
            ball.AddScaleAnimation(destroyDuration, minScale, delegate () { ball.DestroyBall(); });
        }
    }
    #endregion
}
