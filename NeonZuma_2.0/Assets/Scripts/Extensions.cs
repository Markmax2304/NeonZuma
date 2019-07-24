using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    // TODO: move this Ids to separeted class or something
    private static int nextChainId = 0;
    private static int nextBallId = 0;

    public static int ChainId
    {
        get { return nextChainId++; }
    }

    public static int BallId
    {
        get { return nextBallId++; }
    }

    #region Extension Methods
    /// <summary>
    /// Method for getting balls that's belonged current chain
    /// </summary>
    /// <param name="gameEntity">Chain entity, if not - return null</param>
    /// <param name="sorted">true - sorted list, false - not sorted</param>
    /// <returns>Ball entity list</returns>
    public static List<GameEntity> GetChainedBalls(this GameEntity gameEntity, bool sorted = false)
    {
        if (!gameEntity.hasChainId)
            return null;

        List<GameEntity> balls = Contexts.sharedInstance.game.GetEntitiesWithParentChainId(gameEntity.chainId.value).ToList();

        if (!sorted)
            return balls;

        balls.Sort((ball1, ball2) => ball1.distanceBall.value.CompareTo(ball2.distanceBall.value));
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

        if (!sorted)
            return chains;

        chains.Sort((chain1, chain2) => CompareChain(chain1, chain2));
        chains.Reverse();
        return chains;
    }
    #endregion

    #region Private Methods
    private static int CompareChain(GameEntity chain1, GameEntity chain2)
    {
        var firstBall1 = chain1.GetChainedBalls(true).First();
        var firstBall2 = chain2.GetChainedBalls(true).First();

        return firstBall1.distanceBall.value.CompareTo(firstBall2.distanceBall.value);
    }
    #endregion
}
