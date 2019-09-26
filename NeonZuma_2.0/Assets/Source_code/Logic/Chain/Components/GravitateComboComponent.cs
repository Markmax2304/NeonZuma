using UnityEngine;
using Entitas;

/// <summary>
/// Количество сработаных подряд притягиваний на данной цепи. 
/// Предназначен для расчёта комбо, а также модификатора скорости притяжения
/// </summary>
[Game]
public class GravitateComboComponent : IComponent
{
    public int value;
}
