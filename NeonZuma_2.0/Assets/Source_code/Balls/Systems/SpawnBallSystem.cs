using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entitas;
using PathCreation;

public class SpawnBallSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private PoolObjectKeeper pool;
    private float offsetBetweenBalls;

    public SpawnBallSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        pool = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
        offsetBetweenBalls = _contexts.game.levelConfig.value.offsetBetweenBalls;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        GameEntity[] lastBalls = _contexts.game.GetEntities(GameMatcher.LastBall);

        for (int i = 0; i < entities.Count; i++)
        {
            var lastChain = entities[i].GetChains(true).Last();

            float distance = 0;

            for (int j = 0; j < lastBalls.Length; j++)
            {
                if (lastBalls[j].parentChainId.value == lastChain.chainId.value)
                {
                    distance = lastBalls[j].distanceBall.value - offsetBetweenBalls;
                    lastBalls[j].isLastBall = false;
                }
            }

            Transform ball = pool.RealeseObject().transform;
            ColorBall colorType = entities[i].randomizer.value.GetRandomColorType();

            GameEntity entityBall = _contexts.game.CreateEntity();
            entityBall.AddBallId(Extensions.BallId);
            entityBall.AddDistanceBall(distance);
            entityBall.AddTransform(ball);
            entityBall.AddSprite(ball.GetComponent<SpriteRenderer>());
            entityBall.AddColor(colorType);
            entityBall.AddParentChainId(lastChain.chainId.value);
            entityBall.isLastBall = true;

            ball.tag = Constants.BALL_TAG;
            ball.gameObject.Link(entityBall, _contexts.game);

            entities[i].isTimeToSpawn = false;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isTimeToSpawn && entity.hasPathCreator && entity.hasTrackId;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.TrackId, GameMatcher.PathCreator, GameMatcher.TimeToSpawn));
    }
}
