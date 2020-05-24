using UnityEngine;
using Entitas;

public class InitializeBotSystem : IInitializeSystem, ITearDownSystem
{
    private Contexts contexts;
    private LevelConfig config;

    public InitializeBotSystem(Contexts contexts)
    {
        this.contexts = contexts;
        config = this.contexts.global.levelConfig.value;
    }

    public void Initialize()
    {
        GameEntity botEntity = contexts.game.CreateEntity();
        botEntity.isBot = true;

        GameObject bot = GameObject.Instantiate(config.botPrefab);
        bot.transform.position = config.botStartPosition;
        botEntity.AddTransform(bot.transform);

        contexts.manage.startPlayEvent.value.Add(() => InitializeBotState(botEntity));
    }

    public void TearDown()
    {
        var botEntities = contexts.game.GetEntities(GameMatcher.Bot);

        foreach (var bot in botEntities)
        {
            if (bot.hasProjectileInstance)
                bot.projectileInstance.value.DestroyBall();

            var botTransform = bot.transform.value;
            GameObject.Destroy(botTransform.gameObject);
            bot.Destroy();
        }
    }

    #region Private Methods

    private void InitializeBotState(GameEntity botEntity)
    {
        botEntity.AddBotState(BotStateType.Idle);
        botEntity.isRequiredTask = true;
    }

    #endregion Private Methods
}
