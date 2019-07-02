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
            .Add(new InitializePlayerSystem(contexts))
            .Add(new UpdateDeltaTimeSystem(contexts))

            //Input
            .Add(new TouchHandleSystem(contexts))

            //Player
            .Add(new RotatePlayerSystem(contexts))
            .Add(new ShootPlayerSystem(contexts))

            //Spawn
            .Add(new CheckSpawnBallSystem(contexts))
            .Add(new SpawnBallSystem(contexts))

            //Movement
            .Add(new UpdateBallDistanceBySpeedSystem(contexts))
            .Add(new ChangeBallPositionOnPathSystem(contexts))
            .Add(new ShootingForceSystem(contexts))

            //CleanUp
            .Add(new DestroyInputEntityHandleSystem(contexts))
            .Add(new DestroyGameEntityHandleSystem(contexts))
            ;
    }
}
