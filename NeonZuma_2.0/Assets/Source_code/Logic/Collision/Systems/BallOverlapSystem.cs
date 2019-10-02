using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

/// <summary>
/// Логика определения Overlap коллизий
/// </summary>
public class BallOverlapSystem : IExecuteSystem, IInitializeSystem
{
    private Contexts _contexts;
    private float overlapRadius;
    private LayerMask mask;
    private Collider2D[] hits;

    public BallOverlapSystem(Contexts contexts)
    {
        _contexts = contexts;
        mask = LayerMask.GetMask("Balls");
        hits = new Collider2D[4];
    }

    public void Initialize()
    {
        overlapRadius = _contexts.global.levelConfig.value.ballDiametr / 2f;
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
#if UNITY_EDITOR
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to create collision entity. Hit entity is null", TypeLogMessage.Error, true, GetType());
#endif
                return;
            }

            if (hitEntity == ball)
                continue;

            // chain edges collision stuff
            if (IsChainContactCollision(ball, hitEntity))
            {
#if UNITY_EDITOR
                if (_contexts.global.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage(string.Format(" ___ Creating collision with type - {0}, handler - {1}, collider - {2}",
                        TypeCollision.ChainContact.ToString(), ball.ToString(), hitEntity.ToString()), TypeLogMessage.Trace, false, GetType());
                }
#endif
                Contexts.sharedInstance.input.CreateEntity()
                    .AddCollision(TypeCollision.ChainContact, ball, hitEntity);
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
