//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly AddedBallComponent addedBallComponent = new AddedBallComponent();

    public bool isAddedBall {
        get { return HasComponent(GameComponentsLookup.AddedBall); }
        set {
            if (value != isAddedBall) {
                var index = GameComponentsLookup.AddedBall;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : addedBallComponent;

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

    static Entitas.IMatcher<GameEntity> _matcherAddedBall;

    public static Entitas.IMatcher<GameEntity> AddedBall {
        get {
            if (_matcherAddedBall == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AddedBall);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAddedBall = matcher;
            }

            return _matcherAddedBall;
        }
    }
}