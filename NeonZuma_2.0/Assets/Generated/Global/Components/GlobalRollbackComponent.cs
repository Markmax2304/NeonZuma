//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GlobalContext {

    public GlobalEntity rollbackEntity { get { return GetGroup(GlobalMatcher.Rollback).GetSingleEntity(); } }

    public bool isRollback {
        get { return rollbackEntity != null; }
        set {
            var entity = rollbackEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isRollback = true;
                } else {
                    entity.Destroy();
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GlobalEntity {

    static readonly RollbackComponent rollbackComponent = new RollbackComponent();

    public bool isRollback {
        get { return HasComponent(GlobalComponentsLookup.Rollback); }
        set {
            if (value != isRollback) {
                var index = GlobalComponentsLookup.Rollback;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : rollbackComponent;

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
public sealed partial class GlobalMatcher {

    static Entitas.IMatcher<GlobalEntity> _matcherRollback;

    public static Entitas.IMatcher<GlobalEntity> Rollback {
        get {
            if (_matcherRollback == null) {
                var matcher = (Entitas.Matcher<GlobalEntity>)Entitas.Matcher<GlobalEntity>.AllOf(GlobalComponentsLookup.Rollback);
                matcher.componentNames = GlobalComponentsLookup.componentNames;
                _matcherRollback = matcher;
            }

            return _matcherRollback;
        }
    }
}
