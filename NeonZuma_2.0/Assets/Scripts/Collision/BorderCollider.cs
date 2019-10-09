using UnityEngine;

public class BorderCollider : CollisionEmitter
{
    private Vector2 upperRightPoint;
    private Vector2 lowerLeftPoint;

    public void Awake()
    {
        Camera mainCamera = Camera.main;
        EdgeCollider2D edge = GetComponent<EdgeCollider2D>();

        Vector2[] screenVertices = new Vector2[5];
        screenVertices[0] = mainCamera.ScreenToWorldPoint(new Vector2(0, 0));
        screenVertices[1] = mainCamera.ScreenToWorldPoint(new Vector2(0, mainCamera.pixelHeight));
        screenVertices[2] = mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight));
        screenVertices[3] = mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, 0));
        screenVertices[4] = mainCamera.ScreenToWorldPoint(new Vector2(0, 0));

        lowerLeftPoint = screenVertices[0];
        upperRightPoint = screenVertices[2];

        edge.points = screenVertices;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.PROJECTILE_TAG) || collision.CompareTag(Constants.BALL_TAG))
        {
            if (IsOutScreen(collision.transform.position))
            {
                CreateCollisionInputEntity(TypeCollision.OutBorder, gameObject, collision.gameObject);
            }
            else
            {
                CreateCollisionInputEntity(TypeCollision.InBorder, gameObject, collision.gameObject);
            }
        }
    }

    #region Private Methods
    private bool IsOutScreen(Vector2 pos)
    {
        return pos.x <= lowerLeftPoint.x || pos.x >= upperRightPoint.x
            || pos.y <= lowerLeftPoint.y || pos.y >= upperRightPoint.y;
    }
    #endregion
}
