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
            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Destroy balls with goupId {balls[0].groupDestroy.value}", TypeLogMessage.Trace, false, GetType());
            }

            var chain = _contexts.game.GetEntitiesWithChainId(balls.First().parentChainId.value).FirstOrDefault();
            if (chain == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to destroying grouped balls. Chain is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if (track == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to mark for recover chain speed during destroying balls. Track request return null",
                    TypeLogMessage.Error, true, GetType());
                continue;
            }

            // score stuff
            _contexts.manage.CreateEntity().AddScorePiece(balls.Count());

            DestroyBalls(balls);

            if (chain.GetChainedBalls() == null)
            {
                if (_contexts.manage.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ All of chain balls is destroyed. Destroy the chain: {chain.ToString()}", 
                        TypeLogMessage.Trace, false, GetType());
                }

                chain.Destroy();
            }
            else
            {
                // TODO MAYBE: change cut mark to exacter definition, like place where it should be cutted
                if (_contexts.manage.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Mark chain for cutting", TypeLogMessage.Trace, false, GetType());
                }

                chain.isCut = true;
            }

            track.isUpdateSpeed = true;

            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Mark track for updating speed", TypeLogMessage.Trace, false, GetType());
            }
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
            ball.AddScaleAnimation(destroyDuration, minScale, delegate () { ball.DestroyBall(); });
        }
    }
    #endregion
}
