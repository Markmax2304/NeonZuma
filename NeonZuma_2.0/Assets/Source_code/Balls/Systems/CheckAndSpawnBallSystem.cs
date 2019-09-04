using System.Linq;

using UnityEngine;
using Entitas;

public class CheckAndSpawnBallSystem : IExecuteSystem
{
    private Contexts _contexts;
    private float ballDiametr;
    private PoolObjectKeeper pool;
    private Vector3 normalScale;

    private const int clockOverflow = 4;          // increase performance
    private int clock = clockOverflow;

    public CheckAndSpawnBallSystem(Contexts contexts)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
        pool = PoolManager.instance.GetObjectPoolKeeper(TypeObjectPool.Ball);
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
        normalScale = _contexts.game.levelConfig.value.normalScale;
    }

    public void Execute()
    {
        if (clock++ != clockOverflow)
            return;
        else
            clock = 0;

        var tracks = _contexts.game.GetEntities(GameMatcher.TrackId);

        for(int i = 0; i < tracks.Length; i++)
        {
            if (!tracks[i].isSpawnAccess)
                continue;

            var lastChain = tracks[i].GetChains(true)?.LastOrDefault();
            var lastBall = lastChain?.GetChainedBalls(true)?.LastOrDefault();

            if (lastBall != null)
            {
                if (lastBall.distanceBall.value >= ballDiametr)
                {
                    bool isCreateNewChain = CheckDistanceToLastBall(lastBall);
                    SpawnBall(tracks[i], lastChain, lastBall, isCreateNewChain);
                }
            }
            else
            {
                SpawnBall(tracks[i], lastChain, lastBall, true);
            }
        }
    }

    #region Private Methods
    private bool CheckDistanceToLastBall(GameEntity lastBall)
    {
        return lastBall == null || lastBall.distanceBall.value > ballDiametr * 3f;
    }

    private void SpawnBall(GameEntity track, GameEntity lastChain, GameEntity lastBall, bool isCreateNewChain)
    {
        float distance;

        if (isCreateNewChain)
        {
            distance = 0;
            lastChain = _contexts.game.CreateEntity();
            lastChain.AddChainId(Extensions.ChainId);
            lastChain.AddParentTrackId(track.trackId.value);
            lastChain.AddChainSpeed(_contexts.game.levelConfig.value.followSpeed);

            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Created new chain: {lastChain.ToString()}", TypeLogMessage.Trace, false);
            }
        }
        else
        {
            distance = lastBall != null ? lastBall.distanceBall.value - ballDiametr : 0;
        }

        CreateBall(track, lastChain, distance);
        track.isResetChainEdges = true;

        if (_contexts.manage.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage(" ___ Mark track for updating chain edges", TypeLogMessage.Trace, false);
        }
    }

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

        if (_contexts.manage.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ Created new ball in chain: {entityBall.ToString()}", TypeLogMessage.Trace, false);
        }
    }
    #endregion
}
