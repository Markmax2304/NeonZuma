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
        _systems.Cleanup();
    }

    private void OnDestroy()
    {
        _systems.TearDown();
    }

    Systems CreateSystems(Contexts contexts)
    {
        return new Feature("Game")
            //Initialization
            .Add(new InitializePathSystem(contexts))
            .Add(new InitializePlayerSystem(contexts))
            .Add(new UpdateDeltaTimeSystem(contexts))


            //***Collision***
            //Moving In
            .Add(new EnteringBallsToScreenSystem(contexts))
            //Moving Out
            .Add(new CollisionObjectDestroySystem(contexts))        //разобраться с этими коллизиями
            //Connection
            .Add(new ConnectChainsSystem(contexts))
            .Add(new ProjectileCollidingWithBallSystem(contexts))


            //Inserting
            .Add(new BallInsertedToChainSystem(contexts))
            .Add(new MatchInsertedBallInChainSystem(contexts))
            // Cutting chain
            .Add(new VisualDestroyingBallsSystem(contexts))
            .Add(new CutChainSystem(contexts))


            //Spawn
            .Add(new CheckSpawnBallSystem(contexts))
            .Add(new SpawnBallSystem(contexts))
            //Colors
            .Add(new UpdateColorBallSystem(contexts))
            .Add(new CountBallColorsSystem(contexts))
            //Movement
            .Add(new UpdateBallDistanceBySpeedSystem(contexts))
            .Add(new ChangeBallPositionOnPathSystem(contexts))
            .Add(new ShootingForceSystem(contexts))


            //Updating
            .Add(new SetChainEdgesSystem(contexts))


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
