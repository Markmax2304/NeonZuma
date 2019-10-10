using System;
using UnityEngine;

public class CollisionEmitter : MonoBehaviour
{
    private Contexts contexts;
    private void Start()
    {
        contexts = Contexts.sharedInstance;

        if (gameObject.GetEntityLink() == null)
        {
            var entity = contexts.game.CreateEntity();
            entity.AddTransform(transform);
            gameObject.Link(entity, contexts.game);
        }
    }

    private void OnDestroy()
    {
        if (gameObject.GetEntityLink() != null)
        {
            gameObject.Unlink();
        }
    }

    protected void CreateCollisionInputEntity(TypeCollision type, GameObject handler, GameObject collider)
    {
        var handlerLink = handler.GetEntityLink();
        var colliderLink = collider.GetEntityLink();

        if(handlerLink == null)
        {
            throw new Exception($"Handler link is null. Object - {handler.ToString()}");
        }
        if (colliderLink == null)
        {
            throw new Exception($"Collider link is null. Object - {collider.ToString()}");
        }

        contexts.input.CreateEntity()
            .AddCollision(type, handlerLink.entity, colliderLink.entity);
    }
}
