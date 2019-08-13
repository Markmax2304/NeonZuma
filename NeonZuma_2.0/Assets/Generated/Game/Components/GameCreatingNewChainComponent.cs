//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly CreatingNewChainComponent creatingNewChainComponent = new CreatingNewChainComponent();

    public bool isCreatingNewChain {
        get { return HasComponent(GameComponentsLookup.CreatingNewChain); }
        set {
            if (value != isCreatingNewChain) {
                var index = GameComponentsLookup.CreatingNewChain;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : creatingNewChainComponent;

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

    static Entitas.IMatcher<GameEntity> _matcherCreatingNewChain;

    public static Entitas.IMatcher<GameEntity> CreatingNewChain {
        get {
            if (_matcherCreatingNewChain == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.CreatingNewChain);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherCreatingNewChain = matcher;
            }

            return _matcherCreatingNewChain;
        }
    }
}