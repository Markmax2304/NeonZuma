using System.Collections;
using System.Collections.Generic;
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
    public int value;
    public bool isProjectile;
}
