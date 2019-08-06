using UnityEngine;

public class ProjectileCollider : CollisionEmitter
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag(Constants.PROJECTILE_TAG) && CompareWithTags(collision.gameObject, out string tag))
        {
            if (string.Compare(tag, Constants.BALL_TAG) == 0)
            {
                CreateCollisionInputEntity(CollisionType.Projectile, gameObject, collision.gameObject);
            }
        }
    }
}
