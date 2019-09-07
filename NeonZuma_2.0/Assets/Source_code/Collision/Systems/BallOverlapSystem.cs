using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class BallOverlapSystem : IExecuteSystem
{
    private Contexts _contexts;
    private float overlapRadius;
    private LayerMask mask;
    private Collider2D[] hits;

    public BallOverlapSystem(Contexts contexts)
    {
        _contexts = contexts;
        overlapRadius = _contexts.game.levelConfig.value.ballDiametr / 2f;
        mask = LayerMask.GetMask("Balls");
        hits = new Collider2D[4];
    }

    public void Execute()
    {
        var balls = _contexts.game.GetEntities(GameMatcher.AllOf(GameMatcher.Overlap, GameMatcher.BallId));

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
        for (int i = 0; i < count; i++)
        {
            var hitEntity = hits[i].transform.gameObject.GetEntityLink()?.entity;
            if (hitEntity == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to create collision entity. Hit entity is null", TypeLogMessage.Error, true, GetType());
                return;
            }

            if (hitEntity == ball)
                continue;

            // chain edges collision stuff
            if (IsChainContactCollision(ball, hitEntity))
            {
                if (_contexts.manage.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage(string.Format(" ___ Creating collision with type - {0}, handler - {1}, collider - {2}",
                        CollisionType.ChainContact.ToString(), ball.ToString(), hitEntity.ToString()), TypeLogMessage.Trace, false, GetType());
                }

                Contexts.sharedInstance.input.CreateEntity()
                    .AddCollision(CollisionType.ChainContact, ball, hitEntity);
            }
        }
    }

    private bool IsChainContactCollision(GameEntity frontEdge, GameEntity collider)
    {
        return frontEdge.isFrontEdge && collider.isBackEdge 
            && frontEdge.parentChainId.value != collider.parentChainId.value
            && frontEdge.hasBallId && collider.hasBallId;
    }
    #endregion
}
