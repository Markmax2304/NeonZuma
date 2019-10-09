using System;
using UnityEngine;

public class CollisionEmitter : MonoBehaviour
{
    public string[] targetTags;

    private void Start()
    {
        if (gameObject.GetEntityLink() == null)
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();
            entity.AddTransform(transform);
            gameObject.Link(entity, Contexts.sharedInstance.game);
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

        Contexts.sharedInstance.input.CreateEntity()
            .AddCollision(type, handlerLink.entity, colliderLink.entity);
    }

    //protected bool CompareWithTags(GameObject go, out string tag)
    //{
    //    for(int i = 0; i < targetTags.Length; i++)
    //    {
    //        if (go.CompareTag(targetTags[i]))
    //        {
    //            tag = targetTags[i];
    //            return true;
    //        }
    //    }

    //    tag = string.Empty;
    //    return false;
    //}
}
