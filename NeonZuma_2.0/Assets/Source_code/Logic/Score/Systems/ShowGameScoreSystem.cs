using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class ShowGameScoreSystem : ReactiveSystem<ManageEntity>
{
    private Contexts contexts;
    private LevelConfig config;

    public ShowGameScoreSystem(Contexts contexts) : base(contexts.manage)
    {
        this.contexts = contexts;
        config = contexts.global.levelConfig.value;
    }

    protected override void Execute(List<ManageEntity> entities)
    {
        var scoreHandler = contexts.global.scoreHandler;

        scoreHandler.playerScore.text = contexts.manage.totalScore.player.ToString();
        scoreHandler.botScore.text = contexts.manage.totalScore.bot.ToString();
    }

    protected override bool Filter(ManageEntity entity)
    {
        return entity.hasTotalScore;
    }

    protected override ICollector<ManageEntity> GetTrigger(IContext<ManageEntity> context)
    {
        return context.CreateCollector(ManageMatcher.TotalScore);
    }
}
