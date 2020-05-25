using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Комбо уничтожений шаров выстрелом шара подряд
/// Также есть флаг о том, что это уничтожение было именно шаром. Однако, его стоит вынести в отдельный компонент
/// </summary>
[Manage, Unique]
public class ShootInRowComboComponent : IComponent
{
    public NestedComboInfo player;
    public NestedComboInfo bot;

    public struct NestedComboInfo
    {
        public int value;
        public bool isProjectile;
    }
}
