using System.Collections.Generic;

using UnityEngine;
using Entitas;
using DG.Tweening;

public class BotHandleSystem : ReactiveSystem<GameEntity>
{
    private Contexts contexts;
    private LevelConfig config;

    public BotHandleSystem(Contexts contexts) : base(contexts.game)
    {
        this.contexts = contexts;
        config = contexts.global.levelConfig.value;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var botEntity in entities)
        {
            ProcessBotState(botEntity);

            botEntity.isRequiredTask = false;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isRequiredTask && entity.isBot && entity.hasBotState;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.RequiredTask);
    }

    #region Private Methods

    private void ProcessBotState(GameEntity botEntity)
    {
        switch (botEntity.botState.value)
        {
            case BotStateType.Idle:
                botEntity.ReplaceBotState(BotStateType.Scan);
                break;
            case BotStateType.Scan:
                botEntity.ReplaceBotState(BotStateType.Rotate);
                break;
            case BotStateType.Rotate:
                botEntity.ReplaceBotState(BotStateType.Shoot);
                break;
            case BotStateType.Shoot:
                botEntity.ReplaceBotState(BotStateType.Idle);

                Sequence timer = DOTween.Sequence();
                timer.AppendInterval(config.waitingDuration);
                timer.AppendCallback(() => botEntity.isRequiredTask = true);
                timer.Play();

                break;
            default:
                Debug.LogErrorFormat("Unknown bot state - {0}", botEntity.botState.value);
                break;
        }
    }

    #endregion Private Methods
}
