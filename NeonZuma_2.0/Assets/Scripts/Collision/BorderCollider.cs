using UnityEngine;

public class BorderCollider : CollisionEmitter
{
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (CompareWithTags(collision.gameObject, out string tag))
        {
            if (string.Compare(tag, Constants.PROJECTILE_TAG) == 0 || string.Compare(tag, Constants.BALL_TAG) == 0)
            {
                CreateCollisionInputEntity(CollisionType.OutBorder, gameObject, collision.gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareWithTags(collision.gameObject, out string tag) && string.Compare(tag, Constants.BALL_TAG) == 0)
        {
            CreateCollisionInputEntity(CollisionType.InBorder, gameObject, collision.gameObject);
        }
    }
}
