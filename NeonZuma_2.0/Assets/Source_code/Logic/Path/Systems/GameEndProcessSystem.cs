using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class GameEndProcessSystem : ReactiveSystem<GlobalEntity>
{
    private Contexts _contexts;

    public GameEndProcessSystem(Contexts contexts) : base(contexts.global)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GlobalEntity> entities)
    {
        _contexts.global.ReplaceCurrentNormalSpeed(_contexts.global.levelConfig.value.gameOverSpeed);

        var tracks = _contexts.game.GetEntities(GameMatcher.TrackId);
        foreach(var track in tracks)
        {
            track.isSpawnAccess = false;
            track.isUpdateSpeed = true;
        }

        // TODO: invoke some post game over actions
    }

    protected override bool Filter(GlobalEntity entity)
    {
        return entity.isBallReachedEnd;
    }

    protected override ICollector<GlobalEntity> GetTrigger(IContext<GlobalEntity> context)
    {
        return context.CreateCollector(GlobalMatcher.BallReachedEnd);
    }
}
