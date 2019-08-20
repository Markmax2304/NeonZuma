using System.Linq;
using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class UpdateChainSpeedSystem : ReactiveSystem<GameEntity>
{
    private Contexts _contexts;
    private float normalChainSpeed;

    private static Log logger = LogManager.GetCurrentClassLogger();

    public UpdateChainSpeedSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        normalChainSpeed = _contexts.game.levelConfig.value.followSpeed;
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
            lastChain.ReplaceChainSpeed(normalChainSpeed);

            // TODO: add gravitate speed setting to other chains
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
}
