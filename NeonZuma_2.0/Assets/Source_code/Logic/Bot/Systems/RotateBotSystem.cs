using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Entitas;
using PathCreation;
using System.Linq;

public class RotateBotSystem : IExecuteSystem
{
    private Contexts contexts;
    private LevelConfig config;

    public RotateBotSystem(Contexts contexts)
    {
        this.contexts = contexts;
        config = contexts.global.levelConfig.value;
    }

    public void Execute()
    {
        var botEntities = contexts.game.GetEntities(GameMatcher.Bot);

        foreach(var bot in botEntities)
        {
            if (bot.hasBotState && bot.botState.value == BotStateType.Rotate)
            {
                // if target ball is missed
                if (!IsRotated(bot))
                {
                    //Debug.Log("Target is missed");
                    bot.RemoveTargetBall();

                    bot.ReplaceBotState(BotStateType.Idle);
                    bot.isRequiredTask = true;
                    continue;
                }

                // rotate on target
                Transform botTransform = bot.transform.value;
                Vector3 direction = GetDeflectionDirection(bot);

                Debug.DrawRay(bot.transform.value.position, direction * 10f, Color.red);
                Debug.DrawLine(bot.transform.value.position, bot.targetBall.value.transform.value.position, Color.green);

                // TODO: redesign rotation
                botTransform.up = Vector3.Lerp(botTransform.up, direction, contexts.global.deltaTime.value * config.botRotateSpeed);

                // check time to shoot
                if (MatchDirections(botTransform.up, direction))
                {
                    bot.RemoveTargetBall();
                    bot.isRequiredTask = true;
                }
            }
        }
    }

    #region Private Methods

    private bool IsRotated(GameEntity botEntity)
    {
        return botEntity.hasTargetBall
            && botEntity.targetBall.value != null
            && botEntity.targetBall.value.hasBallId;
    }

    private Vector3 GetDeflectionDirection(GameEntity botEntity)
    {
        GameEntity ballEntity = botEntity.targetBall.value;
        var chainEntity = GetChain(ballEntity);
        var trackEntity = GetTrack(chainEntity);

        Vector3 ballPosition = ballEntity.transform.value.position;
        Vector3 botPosition = botEntity.transform.value.position;
        float magnitude = (ballPosition - botPosition).magnitude;

        float timeToReachBall = magnitude / config.forceSpeed;
        float chainSpeed = Mathf.Max(0f, chainEntity.chainSpeed.value);
        float deflectionDistance = ballEntity.distanceBall.value + timeToReachBall * chainSpeed;

        Vector3 deflectionPosition = trackEntity.pathCreator.value.path.GetPointAtDistance(deflectionDistance, EndOfPathInstruction.Stop);
        return (deflectionPosition - botPosition).normalized;
    }

    private GameEntity GetChain(GameEntity ballEntity)
    {
        var chain = contexts.game.GetEntitiesWithChainId(ballEntity.parentChainId.value).FirstOrDefault();
        if (chain == null)
        {
            Debug.LogErrorFormat("Coudn't get chain by id - {0}", ballEntity.parentChainId.value);
        }

        return chain;
    }

    private GameEntity GetTrack(GameEntity chainEntity)
    {
        var track = contexts.game.GetEntitiesWithTrackId(chainEntity.parentTrackId.value).FirstOrDefault();
        if (track == null)
        {
            Debug.LogErrorFormat("Coudn't get track by id - {0}", chainEntity.parentTrackId.value);
        }

        return track;
    }

    private bool MatchDirections(Vector3 dir1, Vector3 dir2)
    {
        return dir1.x - dir2.x <= .001
            && dir1.y - dir2.y <= .001
            && dir1.z - dir2.z <= .001;
    }

    #endregion Private Methods
}
