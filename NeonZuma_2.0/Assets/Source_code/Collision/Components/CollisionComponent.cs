using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

[Input]
public class CollisionComponent : IComponent
{
    public CollisionType type;
    public GameEntity handler;
    public GameEntity collider;
}
