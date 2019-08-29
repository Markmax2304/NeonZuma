using System;
using System.Linq;
using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class MatchInsertedBallInChainSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public MatchInsertedBallInChainSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var checkedBall in entities)
        {
            logger.Trace($" ___ Start to match around inserted ball: {checkedBall.ToString()}");
            GameController.HasRecordToLog = true;

            if (checkedBall == null)
                continue;

            checkedBall.isCheckTargetBall = false;

            var chain = _contexts.game.GetEntitiesWithChainId(checkedBall.parentChainId.value).FirstOrDefault();
            if(chain == null)
            {
                Debug.Log("Failed to match inserted ball. Chain is null");
                logger.Error("Failed to match inserted ball. Chain is null");
                continue;
            }

            var balls = chain.GetChainedBalls(true);
            if(balls == null)
            {
                Debug.Log("Failed to match inserted ball. Balls is null");
                logger.Error("Failed to match inserted ball. Balls is null");
                continue;
            }

            int checkedBallIndex;
            if(!GetBallIndex(balls, checkedBall, out checkedBallIndex))
            {
                Debug.LogError("Failed to match inserted ball. Inserted ball isn't found in ball chain");
                logger.Error("Failed to match inserted ball. Inserted ball isn't found in ball chain");
                continue;
            }

            int count = 0;
            PassBallsWithSameColor(balls, checkedBall.color.value, checkedBallIndex, (ball) => count++);

            if (count >= 3)
            {
                int destroyId = Extensions.DestroyGroupId;
                logger.Trace($" ___ Mark balls for destroying. DestroyGroupId - {destroyId.ToString()}. Count of ball - {count.ToString()}");
                PassBallsWithSameColor(balls, checkedBall.color.value, checkedBallIndex, (ball) => ball.AddGroupDestroy(destroyId));
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isCheckTargetBall && entity.hasBallId && entity.hasParentChainId && !entity.hasGroupDestroy;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.CheckTargetBall);
    }

    #region Private Methods
    private bool GetBallIndex(List<GameEntity> balls, GameEntity ball, out int index)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i] == ball)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private void PassBallsWithSameColor(List<GameEntity> balls, ColorBall insertedColor, int insertedIndex, Action<GameEntity> action)
    {
        for (int i = insertedIndex; i < balls.Count; i++)
        {
            if (balls[i].color.value != insertedColor)
                break;

            action(balls[i]);
        }

        for (int i = insertedIndex - 1; i >= 0; i--)
        {
            if (balls[i].color.value != insertedColor)
                break;

            action(balls[i]);
        }
    }
    #endregion
}
