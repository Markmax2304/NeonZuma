using System.Collections.Generic;
using System.Linq;

using NLog;
using Log = NLog.Logger;

using Entitas;
using UnityEngine;

public class CutChainSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private float ballDiametr;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public CutChainSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var chain in entities)
        {
            logger.Trace($" ___ Start to cut chain: {chain.ToString()} on other chains");
            GameController.HasRecordToLog = true;

            var balls = chain.GetChainedBalls(true);
            if(balls == null)
            {
                Debug.Log("Failed to cut chain. Chain balls is null");
                logger.Error("Failed to cut chain. Chain balls is null");
                continue;
            }

            int firstIndex = 0;

            for(int i = 1; i < balls.Count; i++)
            {
                if(balls[i-1].distanceBall.value - balls[i].distanceBall.value > ballDiametr * 1.1f)
                {
                    logger.Trace($" ___ Found gap in chian between {balls[i - 1].ToString()} and {balls[i].ToString()}");
                    var newChain = CreateEmptyChain(chain.parentTrackId.value);

                    logger.Trace($" ___ Move cutted balls to new chain. Count of balls - {(i - firstIndex).ToString()}");
                    for(int x = firstIndex; x < i; x++)
                    {
                        balls[x].ReplaceParentChainId(newChain.chainId.value);
                    }

                    firstIndex = i;
                }
            }

            chain.isCut = false;

            var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
            if(track == null)
            {
                Debug.Log("Failed to cut chain. Track is null");
                logger.Error("Failed to cut chain. Track is null");
                continue;
            }

            track.isResetChainEdges = true;
            logger.Trace($" ___ Mark track for updating chain edges");
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isCut && entity.hasChainId;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Cut);
    }

    #region Private Methods
    private GameEntity CreateEmptyChain(int trackId)
    {
        var newChain = _contexts.game.CreateEntity();
        newChain.AddChainId(Extensions.ChainId);
        newChain.AddParentTrackId(trackId);
        newChain.AddChainSpeed(0f);
        logger.Trace($" ___ Created new chain {newChain.ToString()}");
        return newChain;
    }
    #endregion
}
