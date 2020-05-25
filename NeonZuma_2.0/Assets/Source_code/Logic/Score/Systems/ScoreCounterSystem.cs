using System.Collections.Generic;

using UnityEngine;
using Entitas;

/// <summary>
/// Логика подсчёта очков на игровом уровне
/// </summary>
public class ScoreCounterSystem : ReactiveSystem<ManageEntity>, IInitializeSystem, ITearDownSystem
{
    private Contexts contexts;
    private int scorePerBall;
    private int decreaseRowCombo;

    public ScoreCounterSystem(Contexts contexts) : base(contexts.manage)
    {
        this.contexts = contexts;
    }

    public void Initialize()
    {
        scorePerBall = contexts.global.levelConfig.value.scorePerBall;
        decreaseRowCombo = contexts.global.levelConfig.value.AmountBallAfterApplyRowCombo;

        contexts.manage.SetTotalScore(0, 0);
        contexts.manage.SetMoveBackCombo(0, 0);

        var info = new ShootInRowComboComponent.NestedComboInfo();
        info.value = 0;
        info.isProjectile = false;
        contexts.manage.SetShootInRowCombo(info, info);
    }

    protected override void Execute(List<ManageEntity> entities)
    {
        foreach (var scoreEntity in entities)
        {
            int player = contexts.manage.totalScore.player;
            int bot = contexts.manage.totalScore.bot;

            switch (scoreEntity.scorePiece.own)
            {
                case OwnType.Player:
                    player += scoreEntity.scorePiece.value * scorePerBall;
                    break;

                case OwnType.Bot:
                    bot += scoreEntity.scorePiece.value * scorePerBall;
                    break;
            }
            

            // TODO: vfx effect
            //Debug.Log($"+ {scoreEntity.scorePiece.value} x {moveBackCombo} + {shootInRowCombo} x {scorePerBall} = {addingToScore}");

            
            contexts.manage.ReplaceTotalScore(player, bot);

            scoreEntity.isDestroyed = true;
        }
    }

    protected override bool Filter(ManageEntity entity)
    {
        return entity.hasScorePiece;
    }

    protected override ICollector<ManageEntity> GetTrigger(IContext<ManageEntity> context)
    {
        return context.CreateCollector(ManageMatcher.ScorePiece);
    }

    public void TearDown()
    {
        // TODO: save result somewhere
        contexts.manage.RemoveTotalScore();
        contexts.manage.RemoveMoveBackCombo();
        contexts.manage.RemoveShootInRowCombo();
    }
}