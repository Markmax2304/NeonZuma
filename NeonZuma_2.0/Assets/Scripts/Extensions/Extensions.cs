using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Entitas;
using DG.Tweening;

public static class Extensions
{
    #region ID stuff
    // TODO: move this Ids to separeted class or something
    private static int nextChainId = 0;
    private static int nextBallId = 0;
    private static int nextGroupId = 0;

    public static int ChainId
    {
        get { return nextChainId++; }
    }

    public static int BallId
    {
        get { return nextBallId++; }
    }

    public static int DestroyGroupId
    {
        get { return nextGroupId++; }
    }
    #endregion

    #region Extension Methods
    /// <summary>
    /// Method for getting balls that's belonged current chain
    /// </summary>
    /// <param name="gameEntity">Chain entity, if not - return null</param>
    /// <param name="sorted">true - sorted list, false - not sorted</param>
    /// <returns>Ball entity list</returns>
    public static List<GameEntity> GetChainedBalls(this GameEntity gameEntity, bool sorted = false, bool reversed = false)
    {
        if (!gameEntity.hasChainId)
            return null;

        var balls = Contexts.sharedInstance.game.GetEntitiesWithParentChainId(gameEntity.chainId.value).ToList();

        if (balls == null || balls.Count == 0)
            return null;

        if (!sorted)
            return balls;

        balls.Sort((ball1, ball2) => ball1.distanceBall.value.CompareTo(ball2.distanceBall.value));

        if (!reversed)
            balls.Reverse();

        return balls;
    }

    /// <summary>
    /// Method for getting chains that's belonged current track
    /// </summary>
    /// <param name="gameEntity">Track entity, if not - return null</param>
    /// <param name="sorted">true - sorted list, false - not sorted</param>
    /// <returns>Chain entity list</returns>
    public static List<GameEntity> GetChains(this GameEntity gameEntity, bool sorted = false)
    {
        if (!gameEntity.hasTrackId)
            return null;

        List<GameEntity> chains = Contexts.sharedInstance.game.GetEntitiesWithParentTrackId(gameEntity.trackId.value).ToList();

        if (chains == null || chains.Count == 0)
            return null;

        if (!sorted)
            return chains;

        chains.Sort((chain1, chain2) => CompareChain(chain1, chain2));
        chains.Reverse();

        return chains;
    }

    public static void DestroyBall(this GameEntity ball)
    {
        // Clean up for components which will be able to attach to ball

        if (ball.hasTransform)
        {
            GameObject obj = ball.transform.value.gameObject;
            DOTween.Kill(obj);
            obj.tag = Constants.UNTAGGED_TAG;
            obj.Unlink();
            var poolObj = obj.GetComponent<PoolingObject>();

            if (poolObj == null)
                GameObject.Destroy(obj);
            else
                poolObj.ReturnToPool();
        }

        ball.Destroy();
    }
    #endregion

    #region Private Methods
    private static int CompareChain(GameEntity chain1, GameEntity chain2)
    {
        var firstBall1 = chain1.GetChainedBalls(true)?.FirstOrDefault();
        var firstBall2 = chain2.GetChainedBalls(true)?.FirstOrDefault();

        if(firstBall1 == null && firstBall2 == null)
        {
            return 0;
        }

        if(firstBall1 == null)
        {
            return -1;
        }

        if(firstBall2 == null)
        {
            return 1;
        }

        return firstBall1.distanceBall.value.CompareTo(firstBall2.distanceBall.value);
    }
    #endregion
}
