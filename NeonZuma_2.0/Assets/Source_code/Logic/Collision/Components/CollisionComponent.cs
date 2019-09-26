using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// <summary>
/// Данные о случившейся коллизии, а именно тип и два объекта учавствующих в столкновении
/// </summary>
[Input]
public class CollisionComponent : IComponent
{
    public TypeCollision type;
    public GameEntity handler;
    public GameEntity collider;
}
