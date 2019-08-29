using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class CountBallColorsSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;

    public CountBallColorsSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        Dictionary<ColorBall, int> colors = _contexts.game.ballColors.value;

        foreach (var entity in entities)
        {
            if (entity.isAddedBall)
            {
                IncrementColorRecords(colors, entity.color.value);
                entity.isAddedBall = false;
            }
            else if (entity.isRemovedBall)
            {
                DecrementColorRecords(colors, entity.color.value);
                entity.isRemovedBall = false;
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasColor && (entity.isAddedBall || entity.isRemovedBall);
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AnyOf(GameMatcher.AddedBall, GameMatcher.RemovedBall));
    }

    #region Private Fields
    private void IncrementColorRecords(Dictionary<ColorBall, int> records, ColorBall color)
    {
        if (records.ContainsKey(color))
            records[color]++;
        else
            records.Add(color, 1);
    }

    private void DecrementColorRecords(Dictionary<ColorBall, int> records, ColorBall color)
    {
        if (records.ContainsKey(color))
        {
            records[color]--;

            if (records[color] == 0)
                records.Remove(color);
        }
        else
        {
            Debug.Log($"Reducing ball number with {color}, that doesn't exist in records");
        }
    }
    #endregion
}
