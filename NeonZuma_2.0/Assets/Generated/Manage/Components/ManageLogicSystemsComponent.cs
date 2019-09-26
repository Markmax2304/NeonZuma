//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class ManageContext {

    public ManageEntity logicSystemsEntity { get { return GetGroup(ManageMatcher.LogicSystems).GetSingleEntity(); } }
    public LogicSystemsComponent logicSystems { get { return logicSystemsEntity.logicSystems; } }
    public bool hasLogicSystems { get { return logicSystemsEntity != null; } }

    public ManageEntity SetLogicSystems(Entitas.Systems newValue) {
        if (hasLogicSystems) {
            throw new Entitas.EntitasException("Could not set LogicSystems!\n" + this + " already has an entity with LogicSystemsComponent!",
                "You should check if the context already has a logicSystemsEntity before setting it or use context.ReplaceLogicSystems().");
        }
        var entity = CreateEntity();
        entity.AddLogicSystems(newValue);
        return entity;
    }

    public void ReplaceLogicSystems(Entitas.Systems newValue) {
        var entity = logicSystemsEntity;
        if (entity == null) {
            entity = SetLogicSystems(newValue);
        } else {
            entity.ReplaceLogicSystems(newValue);
        }
    }

    public void RemoveLogicSystems() {
        logicSystemsEntity.Destroy();
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

    public LogicSystemsComponent logicSystems { get { return (LogicSystemsComponent)GetComponent(ManageComponentsLookup.LogicSystems); } }
    public bool hasLogicSystems { get { return HasComponent(ManageComponentsLookup.LogicSystems); } }

    public void AddLogicSystems(Entitas.Systems newValue) {
        var index = ManageComponentsLookup.LogicSystems;
        var component = (LogicSystemsComponent)CreateComponent(index, typeof(LogicSystemsComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceLogicSystems(Entitas.Systems newValue) {
        var index = ManageComponentsLookup.LogicSystems;
        var component = (LogicSystemsComponent)CreateComponent(index, typeof(LogicSystemsComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveLogicSystems() {
        RemoveComponent(ManageComponentsLookup.LogicSystems);
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

    static Entitas.IMatcher<ManageEntity> _matcherLogicSystems;

    public static Entitas.IMatcher<ManageEntity> LogicSystems {
        get {
            if (_matcherLogicSystems == null) {
                var matcher = (Entitas.Matcher<ManageEntity>)Entitas.Matcher<ManageEntity>.AllOf(ManageComponentsLookup.LogicSystems);
                matcher.componentNames = ManageComponentsLookup.componentNames;
                _matcherLogicSystems = matcher;
            }

            return _matcherLogicSystems;
        }
    }
}
