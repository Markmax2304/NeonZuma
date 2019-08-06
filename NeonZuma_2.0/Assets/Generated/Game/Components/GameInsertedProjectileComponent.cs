//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public InsertedProjectileComponent insertedProjectile { get { return (InsertedProjectileComponent)GetComponent(GameComponentsLookup.InsertedProjectile); } }
    public bool hasInsertedProjectile { get { return HasComponent(GameComponentsLookup.InsertedProjectile); } }

    public void AddInsertedProjectile(GameEntity newChain, GameEntity newFrontBall) {
        var index = GameComponentsLookup.InsertedProjectile;
        var component = (InsertedProjectileComponent)CreateComponent(index, typeof(InsertedProjectileComponent));
        component.chain = newChain;
        component.frontBall = newFrontBall;
        AddComponent(index, component);
    }

    public void ReplaceInsertedProjectile(GameEntity newChain, GameEntity newFrontBall) {
        var index = GameComponentsLookup.InsertedProjectile;
        var component = (InsertedProjectileComponent)CreateComponent(index, typeof(InsertedProjectileComponent));
        component.chain = newChain;
        component.frontBall = newFrontBall;
        ReplaceComponent(index, component);
    }

    public void RemoveInsertedProjectile() {
        RemoveComponent(GameComponentsLookup.InsertedProjectile);
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

    static Entitas.IMatcher<GameEntity> _matcherInsertedProjectile;

    public static Entitas.IMatcher<GameEntity> InsertedProjectile {
        get {
            if (_matcherInsertedProjectile == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.InsertedProjectile);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherInsertedProjectile = matcher;
            }

            return _matcherInsertedProjectile;
        }
    }
}
