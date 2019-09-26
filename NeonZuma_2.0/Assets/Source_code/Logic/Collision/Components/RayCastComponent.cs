using UnityEngine;
using Entitas;

/// <summary>
/// Флаг о том, что эта сущность учавствует в просчёте столкновений по RayCast методу
/// Также есть данные о последнем местоположении для расчёта вектора движения
/// </summary>
[Game]
public class RayCastComponent : IComponent
{
    public Vector3 lastPosition;
}
