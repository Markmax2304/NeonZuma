using Entitas;

/// <summary>
/// Номер для группирования шаров при их уничтожении, чтобы можно было объединить несколько шаров в одну группу
/// </summary>
[Game]
public class GroupDestroyComponent : IComponent
{
    public int value;
}
