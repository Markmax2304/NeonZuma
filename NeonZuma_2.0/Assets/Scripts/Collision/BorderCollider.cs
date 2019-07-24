using UnityEngine;

public class BorderCollider : CollisionEmitter
{
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (CompareWithTags(collision.gameObject))
        {
            CreateCollisionInputEntity(CollisionType.OutBorder, gameObject, collision.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareWithTags(collision.gameObject))
        {
            var entityLink = collision.gameObject.GetEntityLink();
            if (entityLink.entity.hasDistanceBall)
            {
                CreateCollisionInputEntity(CollisionType.InBorder, gameObject, collision.gameObject);
            }
        }
    }
}
