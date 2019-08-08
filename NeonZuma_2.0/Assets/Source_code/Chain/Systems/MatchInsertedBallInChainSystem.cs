using System;
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
        foreach(var insertedBall in entities)
        {
            insertedBall.isInsertedBall = false;
            var chain = _contexts.game.GetEntitiesWithChainId(insertedBall.parentChainId.value).SingleEntity();
            var balls = chain.GetChainedBalls(true);

            int insertedBallIndex;
            if(!GetBallIndex(balls, insertedBall, out insertedBallIndex))
            {
                Debug.LogError("Inserted ball isn't found in ball chain");
                continue;
            }

            int count = 0;
            PassBallsWithSameColor(balls, insertedBall.color.value, insertedBallIndex, (ball) => count++);

            if (count >= 3)
            {
                int destroyId = Extensions.DestroyGroupId;
                PassBallsWithSameColor(balls, insertedBall.color.value, insertedBallIndex, (ball) => ball.AddGroupDestroy(destroyId));
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isInsertedBall && entity.hasParentChainId;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.InsertedBall);
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
