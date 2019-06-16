using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class InitializePlayerSystem : IInitializeSystem
{
    private Contexts _contexts;

    public InitializePlayerSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        GameEntity playerEntity = _contexts.game.CreateEntity();
        playerEntity.isPlayer = true;

        GameObject player = GameObject.Instantiate(_contexts.game.levelConfig.value.playerPrefab);
        playerEntity.AddTransform(player.transform);
    }
}
