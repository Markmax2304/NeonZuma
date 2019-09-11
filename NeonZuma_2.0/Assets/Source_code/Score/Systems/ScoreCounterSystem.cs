using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class ScoreCounterSystem : ReactiveSystem<ManageEntity>, IInitializeSystem
{
    private Contexts _contexts;
    private int scorePerBall;
    private int decreaseRowCombo;

    public ScoreCounterSystem(Contexts contexts) : base(contexts.manage)
    {
        _contexts = contexts;
        scorePerBall = _contexts.global.levelConfig.value.scorePerBall;
        decreaseRowCombo = _contexts.global.levelConfig.value.AmountBallAfterApplyRowCombo;
    }

    public void Initialize()
    {
        _contexts.manage.SetTotalScore(0);
        _contexts.manage.SetMoveBackCombo(0);
        _contexts.manage.SetShootInRowCombo(0, false);
    }

    protected override void Execute(List<ManageEntity> entities)
    {
        foreach (var scoreEntity in entities)
        {
            int moveBackCombo = _contexts.manage.moveBackCombo.value + 1;
            int shootInRowCombo = _contexts.manage.shootInRowCombo.value - decreaseRowCombo;
            if (shootInRowCombo < 0)
                shootInRowCombo = 0;
            int addingToScore = scoreEntity.scorePiece.value * scorePerBall * moveBackCombo + shootInRowCombo * scorePerBall;

            // TODO: vfx effect
            Debug.Log($"+ {scoreEntity.scorePiece.value} x {moveBackCombo} + {shootInRowCombo} x {scorePerBall} = {addingToScore}");

            int totalScore = _contexts.manage.totalScore.value;
            _contexts.manage.ReplaceTotalScore(totalScore + addingToScore);

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
}