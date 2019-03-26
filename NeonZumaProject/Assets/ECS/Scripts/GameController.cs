using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Entitas;

public class GameController : MonoBehaviour
{
    public LevelConfig config;

    private Systems _systems;

    void Start()
    {
        Contexts contexts = Contexts.sharedInstance;

        contexts.game.SetLevelConfig(config);

        _systems = CreateSystems(contexts);

        _systems.Initialize();
    }

    void Update()
    {
        _systems.Execute();
    }

    Systems CreateSystems(Contexts contexts)
    {
        return new Feature("Game")
            .Add(new InitializePathSystem(contexts))

            .Add(new UpdateDeltaTimeSystem(contexts))

            .Add(new CheckSpawnBallSystem(contexts))
            .Add(new SpawnBallSystem(contexts))

            .Add(new UpdateBallDistanceBySpeedSystem(contexts))
            .Add(new ChangeBallPositionOnPathSystem(contexts))
            ;
    }
}
