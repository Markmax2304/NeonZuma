using UnityEngine;

public class BorderCollider : CollisionEmitter
{
    public void Awake()
    {
        Camera mainCamera = Camera.main;
        EdgeCollider2D edge = GetComponent<EdgeCollider2D>();

        Vector2[] corners = new Vector2[5];
        corners[0] = mainCamera.ScreenToWorldPoint(new Vector2(0, 0));
        corners[1] = mainCamera.ScreenToWorldPoint(new Vector2(0, mainCamera.pixelHeight));
        corners[2] = mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight));
        corners[3] = mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, 0));
        corners[4] = mainCamera.ScreenToWorldPoint(new Vector2(0, 0));

        edge.points = corners;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (CompareWithTags(collision.gameObject, out string tag))
        {
            if (string.Compare(tag, Constants.PROJECTILE_TAG) == 0 || string.Compare(tag, Constants.BALL_TAG) == 0)
            {
                CreateCollisionInputEntity(TypeCollision.OutBorder, gameObject, collision.gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareWithTags(collision.gameObject, out string tag) && string.Compare(tag, Constants.BALL_TAG) == 0)
        {
            CreateCollisionInputEntity(TypeCollision.InBorder, gameObject, collision.gameObject);
        }
    }
}
