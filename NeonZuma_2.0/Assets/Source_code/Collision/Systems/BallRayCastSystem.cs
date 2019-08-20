using System.Collections;
using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

public class BallRayCastSystem : IExecuteSystem
{
    private static Log logger = LogManager.GetCurrentClassLogger();

    private Contexts _contexts;
    private float ballDiametr;
    private LayerMask mask;
    private RaycastHit2D[] hits;
    private string[] projectileTags;

    public BallRayCastSystem(Contexts contexts)
    {
        _contexts = contexts;
        ballDiametr = _contexts.game.levelConfig.value.ballDiametr;
        mask = LayerMask.GetMask("Balls");
        hits = new RaycastHit2D[4];
        projectileTags = _contexts.game.levelConfig.value.projectileTags;
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
                ProcessCollision(hits, countHits, ball);
                //Debug.Break();
            }

            //Debug.DrawRay(position, direction * ballDiametr * 3f / 4f, Color.red);
            ball.ReplaceRayCast(position);
        }
    }

    #region Private Methods
    private void ProcessCollision(RaycastHit2D[] hits, int count, GameEntity ball)
    {
        for(int i = 1; i < count; i++)
        {
            var hitGameObject = hits[i].transform.gameObject;

            // projectile collistion stuff
            if (ball.transform.value.CompareTag(Constants.PROJECTILE_TAG) && CompareWithTags(hitGameObject))
            {
                var hitEntity = hitGameObject.GetEntityLink()?.entity;
                if (hitEntity == null)
                {
                    Debug.Log("Failed to create projectile collision entity. Hit entity is null");
                    logger.Error("Failed to create projectile collision entity. Hit entity is null");
                    GameController.HasRecordToLog = true;
                    return;
                }

                logger.Trace($" ___ Creating collision with type - {CollisionType.Projectile.ToString()}, " +
                    "handler - {ball.ToString()}, collider - {hitEntity.ToString()}");
                GameController.HasRecordToLog = true;

                Contexts.sharedInstance.input.CreateEntity()
                    .AddCollision(CollisionType.Projectile, ball, hitEntity);
            }
            // chain edges collision stuff
            else if (ball.transform.value.CompareTag(Constants.FRONT_EDGE_BALL_TAG) && hitGameObject.CompareTag(Constants.BACK_EDGE_BALL_TAG))
            {
                var hitEntity = hitGameObject.GetEntityLink()?.entity;
                if (hitEntity == null)
                {
                    Debug.Log("Failed to create chain collision entity. Hit entity is null");
                    logger.Error("Failed to create chain collision entity. Hit entity is null");
                    return;
                }

                logger.Trace($" ___ Creating collision with type - {CollisionType.ChainContact.ToString()}, " +
                    "handler - {ball.ToString()}, collider - {hitEntity.ToString()}");
                Contexts.sharedInstance.input.CreateEntity()
                    .AddCollision(CollisionType.ChainContact, ball, hitEntity);
            }
        }
    }

    protected bool CompareWithTags(GameObject go)
    {
        for (int i = 0; i < projectileTags.Length; i++)
        {
            if (go.CompareTag(projectileTags[i]))
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}
