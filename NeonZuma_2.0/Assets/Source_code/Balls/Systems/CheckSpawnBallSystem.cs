using System.Linq;

using Entitas;

public class CheckSpawnBallSystem : IExecuteSystem
{
    private Contexts _contexts;
    private float ballDiametr;
    private int countToNewChain = 72;       // 2*0,36*100 // weakness by fps and chain speed
    private int counter = 0;

    private int clock = 4;
    private int clockOverflow = 4;          // divide performance on 4 time

    public CheckSpawnBallSystem(Contexts contexts)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
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
            var lastChain = tracks[i].GetChains(true)?.LastOrDefault();
            var lastBall = lastChain?.GetChainedBalls(true)?.LastOrDefault();

            if (lastBall != null)
            {
                if (lastBall.distanceBall.value >= ballDiametr)
                {
                    if (tracks[i].isCreatingNewChain)
                        continue;

                    if (tracks[i].isTimeToSpawn && ++counter == countToNewChain)
                    {
                        tracks[i].isCreatingNewChain = true;
                        counter = 0;
                    }
                    else
                    {
                        tracks[i].isTimeToSpawn = true;
                    }
                }
            }
            else
            {
                tracks[i].isTimeToSpawn = true;
                tracks[i].isCreatingNewChain = true;
            }
        }
    }
}
