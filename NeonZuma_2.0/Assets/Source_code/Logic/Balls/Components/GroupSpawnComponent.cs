using UnityEngine;
using Entitas;

/// <summary>
/// Количество шаров, которое должно быть заспавнено одновременно
/// </summary>
[Game]
public class GroupSpawnComponent : IComponent
{
    public int count;
}
