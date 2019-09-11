//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GlobalContext {

    public GlobalEntity pointerEntity { get { return GetGroup(GlobalMatcher.Pointer).GetSingleEntity(); } }

    public bool isPointer {
        get { return pointerEntity != null; }
        set {
            var entity = pointerEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isPointer = true;
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

    static readonly PointerComponent pointerComponent = new PointerComponent();

    public bool isPointer {
        get { return HasComponent(GlobalComponentsLookup.Pointer); }
        set {
            if (value != isPointer) {
                var index = GlobalComponentsLookup.Pointer;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : pointerComponent;

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

    static Entitas.IMatcher<GlobalEntity> _matcherPointer;

    public static Entitas.IMatcher<GlobalEntity> Pointer {
        get {
            if (_matcherPointer == null) {
                var matcher = (Entitas.Matcher<GlobalEntity>)Entitas.Matcher<GlobalEntity>.AllOf(GlobalComponentsLookup.Pointer);
                matcher.componentNames = GlobalComponentsLookup.componentNames;
                _matcherPointer = matcher;
            }

            return _matcherPointer;
        }
    }
}