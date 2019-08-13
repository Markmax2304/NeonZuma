using UnityEngine;

// need to rename
public class ProjectileCollider : CollisionEmitter
{
    private static bool isCollided = false;

    public void LateUpdate()
    {
        isCollided = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // projectile collision stuff
        if (gameObject.CompareTag(Constants.PROJECTILE_TAG) && CompareWithTags(collision.gameObject, out string tag))
        {
            CreateCollisionInputEntity(CollisionType.Projectile, gameObject, collision.gameObject);
        }

        // chain edges collistion stuff
        // Only front edge has possibility to register collision
        if (!isCollided && gameObject.CompareTag(Constants.FRONT_EDGE_BALL_TAG) && collision.gameObject.CompareTag(Constants.BACK_EDGE_BALL_TAG))
        {
            isCollided = true;
            CreateCollisionInputEntity(CollisionType.ChainContact, gameObject, collision.gameObject);
        }
    }
}
