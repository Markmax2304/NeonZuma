using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System;

/// <summary>
/// Логика обработки и просчёта указки, когда она активна
/// Длина указки равняется расстоянию до ближайшего шара в направлении выстрела или до конца экрана, если шаров нет на пути
/// </summary>
public class UpdatePointerLengthSystem : IExecuteSystem, IInitializeSystem, ITearDownSystem
{
    private Contexts _contexts;
    private Camera mainCamera;
    private Transform playerTransform;
    private LineRenderer lineRenderer;
    private int mask;
    private RaycastHit2D[] hits;

    private Vector2[] screenCorners;
    private float maxDistance;

    public UpdatePointerLengthSystem(Contexts contexts)
    {
        _contexts = contexts;
        mainCamera = Camera.main;
        mask = LayerMask.GetMask("Balls");
        hits = new RaycastHit2D[4];
        screenCorners = new Vector2[4];
    }
    public void Initialize()
    {
        screenCorners[0] = mainCamera.ScreenToWorldPoint(new Vector2(0, 0));
        screenCorners[1] = mainCamera.ScreenToWorldPoint(new Vector2(0, mainCamera.pixelHeight));
        screenCorners[2] = mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight));
        screenCorners[3] = mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, 0));

        maxDistance = (screenCorners[0] - screenCorners[2]).magnitude * .51f;
        playerTransform = _contexts.game.playerEntity.transform.value;
        lineRenderer = _contexts.game.playerEntity.lineRenderer.value;
    }

    public void Execute()
    {
        // TODO MAYBE: update only when rotate is changed
        if (!_contexts.global.isPointer)
            return;

        Vector3 direction = playerTransform.up;
        Vector3 position = playerTransform.position + direction;

        int countHits = Physics2D.RaycastNonAlloc(position, direction, hits, maxDistance - 1, mask);
        if (countHits > 0)
        {
            // TODO: implement ignoring projectile. It looks a litle strange when pointer is scaling runtime targeting on projectile
            lineRenderer.SetPosition(1, Vector2.up * (hits[0].distance + 1));
        }
        else
        {
            lineRenderer.SetPosition(1, Vector2.up * CalculateDistanceToOutScreen());
        }
    }

    public void TearDown()
    {
        playerTransform = null;
        lineRenderer = null;
    }

    #region Private Methods
    private float CalculateDistanceToOutScreen()
    {
        Vector2 rayBegin = playerTransform.position;
        Vector2 rayEnd = rayBegin + (Vector2)playerTransform.up * maxDistance;
        Vector2 intersectionPoint = Vector2.zero;
        
        for(int i = 0; i <= screenCorners.Length; i++)
        {
            if(HasIntersection(rayBegin, rayEnd, screenCorners[i], screenCorners[(i+1)&3], ref intersectionPoint))
            {
                return (rayBegin - intersectionPoint).magnitude;
            }
        }

        throw new InvalidOperationException("Failed to calculate distance to screen border.");
    }

    private bool HasIntersection(Vector3 v11, Vector3 v12, Vector3 v21, Vector3 v22, ref Vector2 intersect)
    {
        Vector3 cut1 = v12 - v11;
        Vector3 cut2 = v22 - v21;

        Vector3 prod1 = Vector3.Cross(cut1, v21 - v11);
        Vector3 prod2 = Vector3.Cross(cut1, v22 - v11);

        if (CompareSign(prod1.z, prod2.z))
            return false;

        prod1 = Vector3.Cross(cut2, v11 - v21);
        prod2 = Vector3.Cross(cut2, v12 - v21);

        if (CompareSign(prod1.z, prod2.z))
            return false;

        intersect = v11 + cut1 * Mathf.Abs(prod1.z) / Mathf.Abs(prod2.z - prod1.z);
        return true;
    }

    private bool CompareSign(float v1, float v2)
    {
        return (v1 > 0 && v2 > 0) || (v1 < 0 && v2 < 0);
    }
    #endregion
}
