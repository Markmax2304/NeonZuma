using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class GameController : MonoBehaviour
{
    public LevelConfig config;

    private Systems _systems;

    void Start()
    {
        Contexts contexts = Contexts.sharedInstance;

        contexts.game.SetLevelConfig(config);
        contexts.game.SetBallColors(new Dictionary<ColorBall, int>());

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

            //Collision
            .Add(new EnteringBallsToScreenSystem(contexts))
            .Add(new ProjectileCollidingWithBallSystem(contexts))
            .Add(new CollisionObjectDestroySystem(contexts))        //разобраться с этими коллизиями

            //Inserting
            .Add(new BallInsertedToChainSystem(contexts))

            //Spawn
            .Add(new CheckSpawnBallSystem(contexts))
            .Add(new SpawnBallSystem(contexts))
            .Add(new UpdateColorBallSystem(contexts))
            .Add(new CountBallColorsSystem(contexts))

            //Movement
            .Add(new UpdateBallDistanceBySpeedSystem(contexts))
            .Add(new ChangeBallPositionOnPathSystem(contexts))
            .Add(new ShootingForceSystem(contexts))

            //Input
            .Add(new TouchHandleSystem(contexts))

            //Player
            .Add(new RotatePlayerSystem(contexts))
            .Add(new ShootPlayerSystem(contexts))
            .Add(new BallExchangePlayerSystem(contexts))

            //CleanUp
            .Add(new DestroyInputEntityHandleSystem(contexts))
            .Add(new DestroyGameEntityHandleSystem(contexts))
            ;
    }
}
