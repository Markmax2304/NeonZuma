using System.Collections;
using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class BallOverlapSystem : IExecuteSystem
{
    private static Log logger = LogManager.GetCurrentClassLogger();

    private Contexts _contexts;
    private float overlapRadius;
    private LayerMask mask;
    private Collider2D[] hits;

    public BallOverlapSystem(Contexts contexts)
    {
        _contexts = contexts;
        overlapRadius = _contexts.game.levelConfig.value.ballDiametr * .9f / 2f;
        mask = LayerMask.GetMask("Balls");
        hits = new Collider2D[4];
    }

    public void Execute()
    {
        var balls = _contexts.game.GetEntities(GameMatcher.Overlap);

        foreach(var ball in balls)
        {
            Vector3 position = ball.transform.value.position;

            int countHits = Physics2D.OverlapCircleNonAlloc(position, overlapRadius, hits, mask);
            // first hit are always belong to own collider. We are not interested by it
            if (countHits > 1)
            {
                ProcessOverlapCollision(hits, countHits, ball);
            }
        }
    }

    #region Private Methods
    private void ProcessOverlapCollision(Collider2D[] hits, int count, GameEntity ball)
    {
        for (int i = 1; i < count; i++)
        {
            var hitGameObject = hits[i].transform.gameObject;
            var hitEntity = hitGameObject.GetEntityLink()?.entity;
            if (hitEntity == null)
            {
                Debug.Log("Failed to create collision entity. Hit entity is null");
                logger.Error("Failed to create collision entity. Hit entity is null");
                GameController.HasRecordToLog = true;
                return;
            }

            // chain edges collision stuff
            if (IsChainContactCollision(ball, hitEntity))
            {
                logger.Trace(" ___ Creating collision with type - {0}, handler - {1}, collider - {2}",
                    CollisionType.ChainContact.ToString(), ball.ToString(), hitEntity.ToString());
                GameController.HasRecordToLog = true;

                Contexts.sharedInstance.input.CreateEntity()
                    .AddCollision(CollisionType.ChainContact, ball, hitEntity);
            }
        }
    }

    private bool IsChainContactCollision(GameEntity frontEdge, GameEntity collider)
    {
        return frontEdge.isFrontEdge && collider.isBackEdge && frontEdge.parentChainId.value != collider.parentChainId.value
            && frontEdge.hasBallId && collider.hasBallId;
    }
    #endregion
}
