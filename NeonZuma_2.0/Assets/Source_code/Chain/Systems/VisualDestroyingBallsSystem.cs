using System.Collections.Generic;
using System.Linq;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;
using DG.Tweening;

public class VisualDestroyingBallsSystem : ReactiveSystem<GameEntity>, ICleanupSystem
{
    private Contexts _contexts;
    private Dictionary<int, List<GameEntity>> destroyGroups;
    private float destroyDuration;
    private float minScale;

    private static Log logger = LogManager.GetCurrentClassLogger();

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
            logger.Trace($" ___ Destroy balls with goupId {balls[0].groupDestroy.value}");
            GameController.HasRecordToLog = true;

            var chain = _contexts.game.GetEntitiesWithChainId(balls.First().parentChainId.value).FirstOrDefault();
            if(chain == null)
            {
                Debug.Log("Failed to destroying grouped balls. Chain is null");
                logger.Error("Failed to destroying grouped balls. Chain is null");
                continue;
            }

            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if (track == null)
            {
                Debug.Log("Failed to mark for recover chain speed during destroying balls. Track request return null");
                logger.Error("Failed to mark for recover chain speed during destroying balls. Track request return null");
                continue;
            }

            logger.Trace($" ___ Reduce chain speed to zero. Chain: {chain.ToString()}");
            chain.ReplaceChainSpeed(0f);

            for (int i = 0; i < balls.Count; i++)
            {
                var ball = balls[i];
                ball.RemoveBallId();
                ball.RemoveParentChainId();
                ball.transform.value.tag = Constants.UNTAGGED_TAG;
                ball.AddScaleAnimation(destroyDuration, minScale, delegate () { ball.DestroyBall(); });

                //ball.DestroyBall();
                //ball.transform.value.DOScale(minScale, destroyDuration).onComplete += delegate ()
                //{
                //    ball.isDestroyed = true;
                //};
            }

            if (chain.GetChainedBalls() == null)
            {
                logger.Trace($" ___ All of chain balls is destroyed. Destroy the chain: {chain.ToString()}");
                chain.Destroy();
            }
            else
            {
                // TODO MAYBE: change cut mark to exacter definition, like place where it should be cutted
                logger.Trace($" ___ Mark chain for cutting");
                chain.isCut = true;
            }

            track.isUpdateSpeed = true;
            logger.Trace($" ___ Mark track for updating speed");
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
