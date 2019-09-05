using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class MatchInsertedBallInChainSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    public MatchInsertedBallInChainSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var checkedBall in entities)
        {
            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Start to match around inserted ball: {checkedBall.ToString()}", TypeLogMessage.Trace, false, GetType());
            }

            if (checkedBall == null)
                continue;

            checkedBall.isCheckTargetBall = false;

            var chain = _contexts.game.GetEntitiesWithChainId(checkedBall.parentChainId.value).FirstOrDefault();
            if(chain == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to match inserted ball. Chain is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            var balls = chain.GetChainedBalls(true);
            if(balls == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to match inserted ball. Balls is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            int checkedBallIndex;
            if(!GetBallIndex(balls, checkedBall, out checkedBallIndex))
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to match inserted ball. Inserted ball isn't found in ball chain", TypeLogMessage.Error, true, GetType());
                continue;
            }

            int count = 0;
            PassBallsWithSameColor(balls, checkedBall.color.value, checkedBallIndex, (ball) => count++);

            if (count >= 3)
            {
                int destroyId = Extensions.DestroyGroupId;

                if (_contexts.manage.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Mark balls for destroying. DestroyGroupId - {destroyId.ToString()}. Count of ball - {count.ToString()}",
                        TypeLogMessage.Trace, false, GetType());
                }

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
