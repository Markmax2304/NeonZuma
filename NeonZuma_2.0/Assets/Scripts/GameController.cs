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


            //RayCasting
            .Add(new BallRayCastSystem(contexts))

            //Collision of screen
            .Add(new EnteringBallsToScreenSystem(contexts))
            .Add(new CollisionObjectDestroySystem(contexts))        //разобраться с этими коллизиями
            //Collision of chains
            .Add(new ConnectChainsSystem(contexts))
            .Add(new ProjectileCollidingWithBallSystem(contexts))

            //Inserting
            .Add(new BallInsertedToChainSystem(contexts))
            //Destroying balls in chain
            .Add(new MatchInsertedBallInChainSystem(contexts))
            .Add(new VisualDestroyingBallsSystem(contexts))
            .Add(new CutChainSystem(contexts))

            //Spawn
            .Add(new CheckSpawnBallSystem(contexts))
            .Add(new SpawnBallSystem(contexts))
            
            //Movement
            .Add(new UpdateBallDistanceBySpeedSystem(contexts))
            .Add(new ChangeBallPositionOnPathSystem(contexts))

            //Updating
            .Add(new SetChainEdgesSystem(contexts))
            .Add(new UpdateChainSpeedSystem(contexts))
            //Animation
            //.Add(new FinishMoveAnimationSystem(contexts))
            //.Add(new MoveAnimationControlSystem(contexts))


            //Colors
            .Add(new UpdateColorBallSystem(contexts))
            .Add(new CountBallColorsSystem(contexts))


            //Input
            .Add(new TouchHandleSystem(contexts))
            //Player
            .Add(new RotatePlayerSystem(contexts))
            .Add(new ShootPlayerSystem(contexts))
            .Add(new BallExchangePlayerSystem(contexts))
            //Shooting
            .Add(new ShootingForceSystem(contexts))


            //CleanUp
            .Add(new DestroyInputEntityHandleSystem(contexts))
            .Add(new DestroyGameEntityHandleSystem(contexts))
            ;
    }
}
