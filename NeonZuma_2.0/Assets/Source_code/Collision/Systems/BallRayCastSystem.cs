﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class BallRayCastSystem : IExecuteSystem
{
    private Contexts _contexts;
    private float ballDiametr;
    private LayerMask mask;
    private RaycastHit2D[] hits;

    public BallRayCastSystem(Contexts contexts)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
        mask = LayerMask.GetMask("Balls");
        hits = new RaycastHit2D[4];
    }

    public void Execute()
    {
        var balls = _contexts.game.GetEntities(GameMatcher.RayCast);

        foreach(var ball in balls)
        {
            Vector3 position = ball.transform.value.position;
            Vector3 lastPosition = ball.rayCast.lastPosition;

            if(position == lastPosition)
                continue;

            Vector3 direction = (position - lastPosition).normalized;

            int countHits = Physics2D.CircleCastNonAlloc(position, ballDiametr / 2f, direction, hits, ballDiametr / 4f, mask);
            // first hit are always belong to own collider. We are not interested by it
            if (countHits > 1)
            {
                ProcessRayCastCollision(hits, countHits, ball);
            }

            //Debug.DrawRay(position, direction * ballDiametr * 3f / 4f, Color.red);
            ball.ReplaceRayCast(position);
        }
    }

    #region Private Methods
    private void ProcessRayCastCollision(RaycastHit2D[] hits, int count, GameEntity ball)
    {
        for(int i = 0; i < count; i++)
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

            // projectile collistion stuff
            if (IsProjectileCollision(ball, hitEntity))
            {
                if (_contexts.manage.isDebugAccess)
                {
                    _contexts.manage.CreateEntity()
                        .AddLogMessage(string.Format(" ___ Creating collision with type - {0}, handler - {1}, collider - {2}",
                        CollisionType.Projectile.ToString(), ball.ToString(), hitEntity.ToString()), TypeLogMessage.Trace, false, GetType());
                }

                Contexts.sharedInstance.input.CreateEntity()
                    .AddCollision(CollisionType.Projectile, ball, hitEntity);
            }
        }
    }

    private bool IsProjectileCollision(GameEntity projectile, GameEntity collider)
    {
        return projectile.isProjectile && collider.hasBallId;
    }
    #endregion
}
