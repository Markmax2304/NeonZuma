using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Entitas;

public class SpawnBallSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private PoolObjectKeeper pool;
    private float offsetBetweenBalls;
    private Vector3 normalScale;

    public SpawnBallSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        pool = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
        offsetBetweenBalls = _contexts.game.levelConfig.value.offsetBetweenBalls;
        normalScale = _contexts.game.levelConfig.value.normalScale;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        GameEntity[] lastBalls = _contexts.game.GetEntities(GameMatcher.LastBall);

        for (int i = 0; i < entities.Count; i++)
        {
            GameEntity lastChain = entities[i].GetChains(true).Last();
            float distance = 0;

            for (int j = 0; j < lastBalls.Length; j++)
            {
                if (lastBalls[j].parentChainId.value == lastChain.chainId.value)
                {
                    distance = lastBalls[j].distanceBall.value - offsetBetweenBalls;
                    lastBalls[j].isLastBall = false;
                }
            }

            if (entities[i].isCreatingNewChain)
            {
                distance = 0;
                lastChain = _contexts.game.CreateEntity();
                lastChain.AddChainId(Extensions.ChainId);
                lastChain.AddParentTrackId(entities[i].trackId.value);
                lastChain.AddChainSpeed(_contexts.game.levelConfig.value.followSpeed);
            }

            CreateBall(entities[i], lastChain, distance);

            entities[i].isTimeToSpawn = false;
            entities[i].isCreatingNewChain = false;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isTimeToSpawn && entity.hasPathCreator && entity.hasTrackId && entity.isSpawnAccess;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.TimeToSpawn, GameMatcher.SpawnAccess));
    }

    #region Private Methods
    private void CreateBall(GameEntity track, GameEntity chain, float distance)
    {
        // TODO: if will error - replace zero to some more useful
        Transform ball = pool.RealeseObject(Vector3.zero, Quaternion.identity, normalScale).transform;
        ColorBall colorType = track.randomizer.value.GetRandomColorType();

        GameEntity entityBall = _contexts.game.CreateEntity();
        entityBall.AddBallId(Extensions.BallId);
        entityBall.AddDistanceBall(distance);
        entityBall.AddTransform(ball);
        entityBall.AddSprite(ball.GetComponent<SpriteRenderer>());
        entityBall.AddColor(colorType);
        entityBall.AddParentChainId(chain.chainId.value);
        entityBall.isLastBall = true;

        ball.tag = Constants.BALL_TAG;
        ball.gameObject.Link(entityBall, _contexts.game);
    }
    #endregion
}
