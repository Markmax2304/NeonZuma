using System;
using System.Collections.Generic;

using Entitas;

/// <summary>
/// Список событий которые должны быть обработаны по завершению анимации
/// Если анимация прерывается другой анимацией, то новые пост события идут в этот компонент
/// </summary>
[Game]
public class AnimationInfoComponent : IComponent
{
    // TODO: use Queue insted List
    public List<Action> completeActions;
}
