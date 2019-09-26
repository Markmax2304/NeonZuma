using System.Collections.Generic;
using System.Linq;

using Entitas;
using UnityEngine;

/// <summary>
/// Логика проверки цепи на разрыв и непосредственно самого разрыва
/// </summary>
public class CutChainSystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    private Contexts _contexts;
    private float ballDiametr;

    public CutChainSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        ballDiametr = _contexts.global.levelConfig.value.ballDiametr;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var chain in entities)
        {
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Start to cut chain: {chain.ToString()} on other chains", TypeLogMessage.Trace, false, GetType());
            }

            chain.isCut = false;
            var balls = chain.GetChainedBalls(true);

            if (balls == null)
            {
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                    .AddLogMessage($"Chain is empty. Remove chain entity - {chain.ToString()}", TypeLogMessage.Trace, false, GetType());
                }

                DestroyChain(chain);
            }
            else
            {
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                    .AddLogMessage($"", TypeLogMessage.Trace, false, GetType());
                }

                CutChain(balls, chain);
                UpdateTrack(chain);
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
    private void DestroyChain(GameEntity chain)
    {
        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ All of chain balls is destroyed. Destroy the chain: {chain.ToString()}",
                TypeLogMessage.Trace, false, GetType());
        }

        chain.RemoveParentTrackId();
        chain.RemoveChainId();
        chain.isDestroyed = true;
    }

    private void CutChain(List<GameEntity> balls, GameEntity chain)
    {
        int firstIndex = 0;

        for (int i = 1; i < balls.Count; i++)
        {
            if (balls[i - 1].distanceBall.value - balls[i].distanceBall.value > ballDiametr * 1.1f)
            {
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Found gap in chian between {balls[i - 1].ToString()} and {balls[i].ToString()}",
                        TypeLogMessage.Trace, false, GetType());
                }

                var newChain = CreateEmptyChain(chain.parentTrackId.value);

                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage($" ___ Move cutted balls to new chain. Count of balls - {(i - firstIndex).ToString()}",
                        TypeLogMessage.Trace, false, GetType());
                }

                for (int x = firstIndex; x < i; x++)
                {
                    balls[x].ReplaceParentChainId(newChain.chainId.value);
                }

                firstIndex = i;
            }
        }
    }

    private void UpdateTrack(GameEntity chain)
    {
        var track = _contexts.game.GetEntitiesWithTrackId(chain.parentTrackId.value).FirstOrDefault();
        if (track == null)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage("Failed to cut chain. Track is null", TypeLogMessage.Error, true, GetType());
            return;
        }

        track.isResetChainEdges = true;
        track.isUpdateSpeed = true;

        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ Mark track for updating chain edges", TypeLogMessage.Trace, false, GetType());
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ Mark track for updating speed", TypeLogMessage.Trace, false, GetType());
        }
    }

    private GameEntity CreateEmptyChain(int trackId)
    {
        var newChain = _contexts.game.CreateEntity();
        newChain.AddChainId(Extensions.ChainId);
        newChain.AddParentTrackId(trackId);
        newChain.AddChainSpeed(0f);

        if (_contexts.global.isDebugAccess)
        {
            _contexts.manage.CreateEntity()
                .AddLogMessage($" ___ Created new chain {newChain.ToString()}", TypeLogMessage.Trace, false, GetType());
        }

        return newChain;
    }
    #endregion
}
