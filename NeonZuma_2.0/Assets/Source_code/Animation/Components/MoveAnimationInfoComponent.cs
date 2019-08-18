using System.Collections.Generic;

using Entitas;
using DG.Tweening;

[Game]
public class MoveAnimationInfoComponent : IComponent
{
    public List<TweenCallback> completeActions;
}
