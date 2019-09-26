using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// <summary>
/// Количество доступных взрывов
/// Инкрементится после установки флага взрыва
/// Является своеобразным количество последующих выстреленных шаров, которые будут взрывными
/// </summary>
[Global, Unique]
public class ExplosionCountComponent : IComponent
{
    public int value;
}
