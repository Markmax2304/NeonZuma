using System.Collections.Generic;
using System.Linq;

using Entitas;
using UnityEngine;

public class CutChainSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private float ballDiametr;

    public CutChainSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var chain in entities)
        {
            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Start to cut chain: {chain.ToString()} on other chains", TypeLogMessage.Trace, false, GetType());
            }

            var balls = chain.GetChainedBalls(true);
            if(balls == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to cut chain. Chain balls is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            int firstIndex = 0;

            for(int i = 1; i < balls.Count; i++)
            {
                if(balls[i-1].distanceBall.value - balls[i].distanceBall.value > ballDiametr * 1.1f)
                {
                    if (_contexts.manage.isDebugAccess)
                    {
                        _contexts.manage.CreateEntity()
                            .AddLogMessage($" ___ Found gap in chian between {balls[i - 1].ToString()} and {balls[i].ToString()}", 
                            TypeLogMessage.Trace, false, GetType());
                    }

                    var newChain = CreateEmptyChain(chain.parentTrackId.value);

                    if (_contexts.manage.isDebugAccess)
                    {
                        _contexts.manage.CreateEntity()
                            .AddLogMessage($" ___ Move cutted balls to new chain. Count of balls - {(i - firstIndex).ToString()}",
                            TypeLogMessage.Trace, false, GetType());
                    }

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
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to cut chain. Track is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            track.isResetChainEdges = true;

            if (_contexts.manage.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Mark track for updating chain edges", TypeLogMessage.Trace, false, GetType());
            }
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

        if (_contexts.manage.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ Created new chain {newChain.ToString()}", TypeLogMessage.Trace, false, GetType());
        }

        return newChain;
    }
    #endregion
}
