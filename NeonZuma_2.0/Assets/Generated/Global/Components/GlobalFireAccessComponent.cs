//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GlobalContext {

    public GlobalEntity fireAccessEntity { get { return GetGroup(GlobalMatcher.FireAccess).GetSingleEntity(); } }

    public bool isFireAccess {
        get { return fireAccessEntity != null; }
        set {
            var entity = fireAccessEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isFireAccess = true;
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

    static readonly FireAccessComponent fireAccessComponent = new FireAccessComponent();

    public bool isFireAccess {
        get { return HasComponent(GlobalComponentsLookup.FireAccess); }
        set {
            if (value != isFireAccess) {
                var index = GlobalComponentsLookup.FireAccess;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : fireAccessComponent;

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

    static Entitas.IMatcher<GlobalEntity> _matcherFireAccess;

    public static Entitas.IMatcher<GlobalEntity> FireAccess {
        get {
            if (_matcherFireAccess == null) {
                var matcher = (Entitas.Matcher<GlobalEntity>)Entitas.Matcher<GlobalEntity>.AllOf(GlobalComponentsLookup.FireAccess);
                matcher.componentNames = GlobalComponentsLookup.componentNames;
                _matcherFireAccess = matcher;
            }

            return _matcherFireAccess;
        }
    }
}
