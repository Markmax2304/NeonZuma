using System.Collections.Generic;

using UnityEngine;
using Entitas;
using DG.Tweening;

public class ScanBallTrackSystem : ReactiveSystem<GameEntity>
{
    private Contexts contexts;
    private LevelConfig config;

    private RaycastHit2D[] hits;
    private LayerMask mask;

    private List<GameEntity> appropriateBolls;

    public ScanBallTrackSystem(Contexts contexts) : base(contexts.game)
    {
        this.contexts = contexts;
        config = contexts.global.levelConfig.value;

        hits = new RaycastHit2D[4];
        mask = LayerMask.GetMask("Balls");

        appropriateBolls = new List<GameEntity>();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var botEntity in entities)
        {
            if (RayCastAround(botEntity))
            {
                botEntity.isRequiredTask = true;
            }
            else
            {
                // bad point, but nothing to do
                botEntity.ReplaceBotState(BotStateType.Idle);

                Sequence timer = DOTween.Sequence();
                timer.AppendInterval(config.waitingDuration);
                timer.AppendCallback(() => botEntity.isRequiredTask = true);
                timer.Play();
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isBot && entity.hasBotState && entity.botState.value == BotStateType.Scan;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BotState);
    }

    #region Private Methods

    private bool RayCastAround(GameEntity botEntity)
    {
        int rayCount = config.scanRayAmount;
        Vector3 startPosition = botEntity.transform.value.position;

        appropriateBolls.Clear();

        for (int i = 0; i < rayCount; i++)
        {
            float degree = i * 2 * Mathf.PI / rayCount;
            Vector3 rayDirection = new Vector3(Mathf.Sin(degree), Mathf.Cos(degree), 0f);

            if (RayCastSingle(botEntity, startPosition, rayDirection, out GameEntity targetBall))
            {
                appropriateBolls.Add(targetBall);
            }
        }

        if (appropriateBolls.Count > 0)
        {
            int randomIndex = Random.Range(0, appropriateBolls.Count);
            botEntity.AddTargetBall(appropriateBolls[randomIndex]);
            return true;
        }

        return false;
    }

    private bool RayCastSingle(GameEntity botEntity, Vector3 pos, Vector3 dir, out GameEntity appropriateBall)
    {
        int countHits = Physics2D.RaycastNonAlloc(pos, dir, hits, Mathf.Infinity, mask);
        //Debug.DrawRay(pos, dir * 10f, Color.red, 1f);

        if (countHits > 0)
        {
            for (int i = 0; i < countHits; i++)
            {
                //Debug.Log(hits[i].collider.gameObject.ToString());
                GameEntity ballEntity = hits[i].collider.gameObject.GetEntityLink()?.entity;

                if (ballEntity == null || !ballEntity.hasBallId)
                    continue;

                GameEntity projectileEntity = botEntity.projectileInstance.value;
                if (projectileEntity.color.value == ballEntity.color.value)
                {
                    appropriateBall = ballEntity;
                    return true;
                }
            }
        }

        appropriateBall = null;
        return false;
    }

    #endregion Private Methods
}
