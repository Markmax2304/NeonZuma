using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Entitas;

public class SpawnBallSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private PoolObjectKeeper pool;
    private float ballDiametr;
    private Vector3 normalScale;

    public SpawnBallSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        pool = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
        normalScale = _contexts.game.levelConfig.value.normalScale;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var track in entities)
        {
            float distance = 0;
            var lastChain = track.GetChains(true)?.LastOrDefault();
            var lastBall = lastChain?.GetChainedBalls(true)?.LastOrDefault();

            if(lastBall != null)
            {
                distance = lastBall.distanceBall.value - ballDiametr;
            }

            if (track.isCreatingNewChain)
            {
                distance = 0;
                lastChain = _contexts.game.CreateEntity();
                lastChain.AddChainId(Extensions.ChainId);
                lastChain.AddParentTrackId(track.trackId.value);
                lastChain.AddChainSpeed(_contexts.game.levelConfig.value.followSpeed);
            }

            CreateBall(track, lastChain, distance);

            track.isTimeToSpawn = false;
            track.isCreatingNewChain = false;
            track.isResetChainEdges = true;
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

        ball.tag = Constants.BALL_TAG;
        ball.gameObject.Link(entityBall, _contexts.game);
    }
    #endregion
}
