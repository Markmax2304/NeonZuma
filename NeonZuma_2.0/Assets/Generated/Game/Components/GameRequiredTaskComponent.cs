//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly RequiredTaskComponent requiredTaskComponent = new RequiredTaskComponent();

    public bool isRequiredTask {
        get { return HasComponent(GameComponentsLookup.RequiredTask); }
        set {
            if (value != isRequiredTask) {
                var index = GameComponentsLookup.RequiredTask;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : requiredTaskComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
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

    static Entitas.IMatcher<GameEntity> _matcherRequiredTask;

    public static Entitas.IMatcher<GameEntity> RequiredTask {
        get {
            if (_matcherRequiredTask == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.RequiredTask);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherRequiredTask = matcher;
            }

            return _matcherRequiredTask;
        }
    }
}
