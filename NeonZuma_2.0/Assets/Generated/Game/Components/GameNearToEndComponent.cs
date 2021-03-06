//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly NearToEndComponent nearToEndComponent = new NearToEndComponent();

    public bool isNearToEnd {
        get { return HasComponent(GameComponentsLookup.NearToEnd); }
        set {
            if (value != isNearToEnd) {
                var index = GameComponentsLookup.NearToEnd;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : nearToEndComponent;

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

    static Entitas.IMatcher<GameEntity> _matcherNearToEnd;

    public static Entitas.IMatcher<GameEntity> NearToEnd {
        get {
            if (_matcherNearToEnd == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.NearToEnd);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherNearToEnd = matcher;
            }

            return _matcherNearToEnd;
        }
    }
}
