using UnityEngine;

public class ProjectileCollider : CollisionEmitter
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag(Constants.PROJECTILE_TAG) && CompareWithTags(collision.gameObject))
        {
            var entityLink = collision.gameObject.GetEntityLink();
            if (entityLink.entity.hasDistanceBall)
            {
                CreateCollisionInputEntity(CollisionType.Projectile, gameObject, collision.gameObject);
            }
        }
    }
}
