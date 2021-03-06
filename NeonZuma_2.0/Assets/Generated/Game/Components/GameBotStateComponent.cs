//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public BotStateComponent botState { get { return (BotStateComponent)GetComponent(GameComponentsLookup.BotState); } }
    public bool hasBotState { get { return HasComponent(GameComponentsLookup.BotState); } }

    public void AddBotState(BotStateType newValue) {
        var index = GameComponentsLookup.BotState;
        var component = (BotStateComponent)CreateComponent(index, typeof(BotStateComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceBotState(BotStateType newValue) {
        var index = GameComponentsLookup.BotState;
        var component = (BotStateComponent)CreateComponent(index, typeof(BotStateComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveBotState() {
        RemoveComponent(GameComponentsLookup.BotState);
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

    static Entitas.IMatcher<GameEntity> _matcherBotState;

    public static Entitas.IMatcher<GameEntity> BotState {
        get {
            if (_matcherBotState == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.BotState);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherBotState = matcher;
            }

            return _matcherBotState;
        }
    }
}
