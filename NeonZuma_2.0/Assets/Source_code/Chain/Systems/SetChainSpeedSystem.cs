using System.Linq;
using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class SetChainSpeedSystem : ReactiveSystem<GameEntity>, IInitializeSystem, ICleanupSystem
{
    private Contexts _contexts;
    private float normalChainSpeed;
    private float gravitateSpeed;
    private float increaseSpeed;

    private static Log logger = LogManager.GetCurrentClassLogger();
    private static Dictionary<int, List<GameEntity>> chainDict = new Dictionary<int, List<GameEntity>>();

    public SetChainSpeedSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        normalChainSpeed = _contexts.game.levelConfig.value.followSpeed;
        gravitateSpeed = _contexts.game.levelConfig.value.gravitateSpeed;
        increaseSpeed = _contexts.game.levelConfig.value.increaseMoveBackFactor;
    }

    public void Initialize()
    {
        float oldNormalSpeed = normalChainSpeed;
        normalChainSpeed = _contexts.game.levelConfig.value.startSpeed;

        var tracks = _contexts.game.GetEntities(GameMatcher.TrackId);
        foreach (var track in tracks)
        {
            track.isUpdateSpeed = true;
        }

        var counterEntity = _contexts.game.CreateEntity();
        counterEntity.AddCounter(_contexts.game.levelConfig.value.startDuration, delegate ()
        {
            logger.Trace($" ___ Recover normal speed. Mark to update speed. And invoke start game event");
            normalChainSpeed = oldNormalSpeed;

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
            logger.Trace($" ___ Update speed of track chains. Track - {track.ToString()}");
            GameController.HasRecordToLog = true;

            track.isUpdateSpeed = false;

            var chains = track.GetChains(true);
            if(chains == null)
            {
                Debug.Log("Failed to update chains speed. Chain collection is null");
                logger.Error("Failed to update chains speed. Chain collection is null");
                continue;
            }

            var lastChain = chains.Last();
            if (!lastChain.hasCounter)
            {
                lastChain.ReplaceChainSpeed(track.isNearToEnd ? normalChainSpeed * .4f : normalChainSpeed);
            }

            // add gravitate speed setting to other chains
            for(int i = 0; i < chains.Count; i++)
            {
                var balls = chains[i].GetChainedBalls(true);
                if(balls == null)
                {
                    Debug.Log("Failed to update chains speed. Balls of chain is null");
                    logger.Error("Failed to update chains speed. Balls of chain is null");
                    return;
                }

                chainDict.Add(chains[i].chainId.value, balls);
            }

            for(int i = 0; i < chains.Count - 1; i++)
            {
                SetChainSpeedByEdgeBalls(chains[i], chains[i + 1]);
            }
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
            frontChain.ReplaceChainSpeed(-gravitateSpeed * (1 + increaseSpeed * _contexts.game.moveBackCombo.value));
        }
    }
    #endregion
}
