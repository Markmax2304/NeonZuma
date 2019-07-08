using System.Collections.Generic;
using UnityEngine;
using Entitas;
using PathCreation;

public class SpawnBallSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private PoolObjectKeeper pool;

    public SpawnBallSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        pool = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        for(int i = 0; i < entities.Count; i++) {
            GameEntity[] lastBalls = _contexts.game.GetEntities(GameMatcher.LastBall);

            float distance = 0;
            for(int j = 0; j < lastBalls.Length; j++) {
                if(lastBalls[j].trackId.value == entities[i].trackId.value) {
                    distance = lastBalls[j].distanceBall.value - _contexts.game.levelConfig.value.offsetBetweenBalls;
                    lastBalls[j].isLastBall = false;
                }
            }

            PathCreator pathCreator = entities[i].pathCreator.value;
            Transform ball = pool.RealeseObject().transform;
            ColorBall colorType = entities[i].randomizer.value.GetRandomColorType();

            GameEntity entityBall = _contexts.game.CreateEntity();
            entityBall.AddDistanceBall(distance);
            entityBall.AddTransform(ball);
            entityBall.AddSprite(ball.GetComponent<SpriteRenderer>());
            entityBall.AddColor(colorType);
            entityBall.isAddedBall = true;
            entityBall.AddPathCreator(pathCreator);
            entityBall.AddTrackId(entities[i].trackId.value);
            entityBall.isLastBall = true;

            entities[i].isTimeToSpawn = false;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isTimeToSpawn && entity.hasPathCreator && entity.isTrack;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Track, GameMatcher.PathCreator, GameMatcher.TimeToSpawn));
    }
}
