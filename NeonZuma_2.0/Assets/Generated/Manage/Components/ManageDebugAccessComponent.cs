//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class ManageContext {

    public ManageEntity debugAccessEntity { get { return GetGroup(ManageMatcher.DebugAccess).GetSingleEntity(); } }

    public bool isDebugAccess {
        get { return debugAccessEntity != null; }
        set {
            var entity = debugAccessEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isDebugAccess = true;
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
public partial class ManageEntity {

    static readonly DebugAccessComponent debugAccessComponent = new DebugAccessComponent();

    public bool isDebugAccess {
        get { return HasComponent(ManageComponentsLookup.DebugAccess); }
        set {
            if (value != isDebugAccess) {
                var index = ManageComponentsLookup.DebugAccess;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : debugAccessComponent;

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
public sealed partial class ManageMatcher {

    static Entitas.IMatcher<ManageEntity> _matcherDebugAccess;

    public static Entitas.IMatcher<ManageEntity> DebugAccess {
        get {
            if (_matcherDebugAccess == null) {
                var matcher = (Entitas.Matcher<ManageEntity>)Entitas.Matcher<ManageEntity>.AllOf(ManageComponentsLookup.DebugAccess);
                matcher.componentNames = ManageComponentsLookup.componentNames;
                _matcherDebugAccess = matcher;
            }

            return _matcherDebugAccess;
        }
    }
}
