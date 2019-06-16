//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly UpdateDistanceComponent updateDistanceComponent = new UpdateDistanceComponent();

    public bool isUpdateDistance {
        get { return HasComponent(GameComponentsLookup.UpdateDistance); }
        set {
            if (value != isUpdateDistance) {
                var index = GameComponentsLookup.UpdateDistance;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : updateDistanceComponent;

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

    static Entitas.IMatcher<GameEntity> _matcherUpdateDistance;

    public static Entitas.IMatcher<GameEntity> UpdateDistance {
        get {
            if (_matcherUpdateDistance == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.UpdateDistance);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherUpdateDistance = matcher;
            }

            return _matcherUpdateDistance;
        }
    }
}