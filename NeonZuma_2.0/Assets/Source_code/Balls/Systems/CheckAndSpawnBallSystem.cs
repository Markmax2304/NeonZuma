using System.Linq;

using UnityEngine;
using Entitas;

public class CheckAndSpawnBallSystem : IExecuteSystem
{
    private Contexts _contexts;
    private float ballDiametr;
    private PoolObjectKeeper pool;
    private Vector3 normalScale;

    private const int countBallForCutting = 3;
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

            int countSpawn = GetCountSpawnBalls(tracks[i]);

            var lastChain = tracks[i].GetChains(true)?.LastOrDefault();
            var lastBall = lastChain?.GetChainedBalls(true)?.LastOrDefault();

            if (lastBall != null)
            {
                if (lastBall.distanceBall.value >= ballDiametr)
                {
                    bool isCreateNewChain = CheckDistanceToLastBall(lastBall);
                    SpawnBalls(tracks[i], lastChain, lastBall, countSpawn, isCreateNewChain);
                }
            }
            else
            {
                SpawnBalls(tracks[i], lastChain, lastBall, countSpawn, true);
            }
        }
    }

    #region Private Methods
    private bool CheckDistanceToLastBall(GameEntity lastBall)
    {
        return lastBall == null || lastBall.distanceBall.value > ballDiametr * countBallForCutting;
    }

    private int GetCountSpawnBalls(GameEntity track)
    {
        if (!track.hasGroupSpawn)
        {
            return 1;
        }
        else
        {
            int countSpawn = track.groupSpawn.count;
            track.RemoveGroupSpawn();
            return countSpawn;
        }
    }

    private void SpawnBalls(GameEntity track, GameEntity lastChain, GameEntity lastBall, int count, bool isCreateNewChain)
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
                    .AddLogMessage($" ___ Created new chain: {lastChain.ToString()}", TypeLogMessage.Trace, false, GetType());
            }
        }
        else
        {
            distance = lastBall != null ? lastBall.distanceBall.value - ballDiametr : 0;
        }

        for(int i = 0; i < count; i++)
        {
            CreateBall(track, lastChain, distance - ballDiametr * i);
        }

        track.isResetChainEdges = true;
        if (_contexts.manage.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage(" ___ Mark track for updating chain edges", TypeLogMessage.Trace, false, GetType());
        }
    }

    private void CreateBall(GameEntity track, GameEntity chain, float distance)
    {
        // TODO: if will error - replace zero to some more useful
        //var pathCreator           // continue point
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
                .AddLogMessage($" ___ Created new ball in chain: {entityBall.ToString()}", TypeLogMessage.Trace, false, GetType());
        }
    }
    #endregion
}
