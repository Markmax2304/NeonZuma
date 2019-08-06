using System.Linq;

using Entitas;

public class CheckSpawnBallSystem : IExecuteSystem
{
    private Contexts _contexts;
    private float offsetBetweenBalls;
    private int countToNewChain = 72;       // 2*0,36*100 // weakness by fps and chain speed
    private int counter = 0;

    public CheckSpawnBallSystem(Contexts contexts)
    {
        _contexts = contexts;
        offsetBetweenBalls = _contexts.game.levelConfig.value.offsetBetweenBalls;
    }

    public void Execute()
    {
        GameEntity[] balls = _contexts.game.GetEntities(GameMatcher.LastBall);

        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i].distanceBall.value >= offsetBetweenBalls)
            {
                var chain = _contexts.game.GetEntitiesWithChainId(balls[i].parentChainId.value).First();
                var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).First();

                if (track.isCreatingNewChain)
                    continue;

                if (track.isTimeToSpawn && ++counter == countToNewChain)
                {
                    track.isCreatingNewChain = true;
                    counter = 0;
                }
                else
                {
                    track.isTimeToSpawn = true;
                }
            }
        }
    }
}
