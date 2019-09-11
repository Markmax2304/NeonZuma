using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class SetChainSpeedSystem : ReactiveSystem<GameEntity>, IInitializeSystem, ICleanupSystem
{
    private Contexts _contexts;

    private static Dictionary<int, List<GameEntity>> chainDict = new Dictionary<int, List<GameEntity>>();

    public SetChainSpeedSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        _contexts.global.SetCurrentNormalSpeed(_contexts.global.levelConfig.value.startSpeed);

        var tracks = _contexts.game.GetEntities(GameMatcher.TrackId);
        foreach (var track in tracks)
        {
            track.isUpdateSpeed = true;
        }

        var counterEntity = _contexts.game.CreateEntity();
        counterEntity.AddCounter(_contexts.global.levelConfig.value.startDuration, delegate ()
        {
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Recover normal speed. Mark to update speed. And invoke start game event", 
                    TypeLogMessage.Trace, false, GetType());
            }

            _contexts.global.ReplaceCurrentNormalSpeed(_contexts.global.levelConfig.value.followSpeed);

            foreach(var track in tracks)
            {
                track.isUpdateSpeed = true;
            }

            var actions = _contexts.manage.startPlayEvent.value;
            foreach(var action in actions)
            {
                action();
            }
            _contexts.manage.RemoveStartPlayEvent();

            counterEntity.isDestroyed = true;
        });
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var track in entities)
        {
            if (_contexts.global.isDebugAccess)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage($" ___ Update speed of track chains. Track - {track.ToString()}", TypeLogMessage.Trace, false, GetType());
            }

            track.isUpdateSpeed = false;

            var chains = track.GetChains(true);
            if(chains == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to update chains speed. Chain collection is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            SetChainsSpeed(track, chains);

            // add gravitate speed setting to other chains
            SetChainGravitateSpeed(chains);
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isUpdateSpeed && entity.hasTrackId;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.UpdateSpeed);
    }

    public void Cleanup()
    {
        chainDict.Clear();
    }

    #region Private Methods
    private void SetChainSpeedByEdgeBalls(GameEntity frontChain, GameEntity backChain)
    {
        var frontBalls = chainDict[frontChain.chainId.value];
        var backBalls = chainDict[backChain.chainId.value];

        if(frontBalls.Last().color.value == backBalls.First().color.value)
        {
            float gravitateSpeed = _contexts.global.levelConfig.value.gravitateSpeed;
            float increaseSpeed = _contexts.global.levelConfig.value.increaseMoveBackFactor;
            frontChain.ReplaceChainSpeed(-gravitateSpeed * (1 + increaseSpeed * _contexts.manage.moveBackCombo.value));
        }
    }

    private void SetChainsSpeed(GameEntity track, List<GameEntity> chains)
    {
        if (_contexts.global.isRollback)
        {
            float rollbackSpeed = _contexts.global.levelConfig.value.rollbackSpeed;
            foreach(var chain in chains)
            {
                chain.ReplaceChainSpeed(-rollbackSpeed);
            }
        }
        else
        {
            var lastChain = chains.Last();
            if (!lastChain.hasCounter)
            {
                float speed = _contexts.global.currentNormalSpeed.value;
                lastChain.ReplaceChainSpeed(track.isNearToEnd ? speed * .4f : speed);
            }

            for(int i = 0; i < chains.Count - 1; i++)
            {
                chains[i].ReplaceChainSpeed(0f);
            }
        }
    }

    private void SetChainGravitateSpeed(List<GameEntity> chains)
    {
        for (int i = 0; i < chains.Count; i++)
        {
            var balls = chains[i].GetChainedBalls(true);
            if (balls == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to update chains speed. Balls of chain is null", TypeLogMessage.Error, true, GetType());
                return;
            }

            chainDict.Add(chains[i].chainId.value, balls);
        }

        for (int i = 0; i < chains.Count - 1; i++)
        {
            SetChainSpeedByEdgeBalls(chains[i], chains[i + 1]);
        }
    }
    #endregion
}
