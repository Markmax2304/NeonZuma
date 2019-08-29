//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public ScaleAnimationComponent scaleAnimation { get { return (ScaleAnimationComponent)GetComponent(GameComponentsLookup.ScaleAnimation); } }
    public bool hasScaleAnimation { get { return HasComponent(GameComponentsLookup.ScaleAnimation); } }

    public void AddScaleAnimation(float newDuration, float newTargetScale, System.Action newPostAction) {
        var index = GameComponentsLookup.ScaleAnimation;
        var component = (ScaleAnimationComponent)CreateComponent(index, typeof(ScaleAnimationComponent));
        component.duration = newDuration;
        component.targetScale = newTargetScale;
        component.postAction = newPostAction;
        AddComponent(index, component);
    }

    public void ReplaceScaleAnimation(float newDuration, float newTargetScale, System.Action newPostAction) {
        var index = GameComponentsLookup.ScaleAnimation;
        var component = (ScaleAnimationComponent)CreateComponent(index, typeof(ScaleAnimationComponent));
        component.duration = newDuration;
        component.targetScale = newTargetScale;
        component.postAction = newPostAction;
        ReplaceComponent(index, component);
    }

    public void RemoveScaleAnimation() {
        RemoveComponent(GameComponentsLookup.ScaleAnimation);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherScaleAnimation;

    public static Entitas.IMatcher<GameEntity> ScaleAnimation {
        get {
            if (_matcherScaleAnimation == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.ScaleAnimation);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherScaleAnimation = matcher;
            }

            return _matcherScaleAnimation;
        }
    }
}