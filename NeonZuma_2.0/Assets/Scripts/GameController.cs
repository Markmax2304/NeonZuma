using System;
using System.IO;
using System.Collections.Generic;

using NLog;
using UnityEngine;
using Entitas;

public class GameController : MonoBehaviour
{
    public bool isDebug = false;
    public LevelConfig config;

    private Systems _systems;

    private static string tempFolder;
    private NLog.Logger logger;

    void Start()
    {
#if UNITY_EDITOR
        InitializeLogger();
        if (isDebug) 
        {
            logger.Trace("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        }
#endif

        Application.targetFrameRate = 60;

        Contexts contexts = Contexts.sharedInstance;

        InitializeSingletonComponents(contexts);
        _systems = CreateSystems(contexts);
        _systems.Initialize();
    }

    void Update()
    {
        try
        {
            _systems.Execute();
            _systems.Cleanup();
        }
        catch (Exception ex)
        {
#if UNITY_EDITOR
            logger.Error(ex, "Failed to process update frame");
#endif
        }
    }

    private void OnDestroy()
    {
        _systems.TearDown();
    }

    private void InitializeSingletonComponents(Contexts contexts)
    {
        contexts.global.SetLevelConfig(config);
        contexts.global.isDebugAccess = isDebug;
        contexts.manage.SetLogicSystems(CreateLogicSystems(contexts));
    }

    private Systems CreateSystems(Contexts contexts)
    {
        return new Feature("Game")
            .Add(new UpdateDeltaTimeSystem(contexts))

            //Level
            .Add(new UploadLevelSystem(contexts))
            .Add(new ExecuteLevelLogicSystem(contexts))         // here locates execution of Logic systems
            .Add(new FinishLevelSystem(contexts))

            // Log recording
            .Add(new RecordLogMessageSystem(contexts))

            //CleanUp
            .Add(new DestroyInputEntityHandleSystem(contexts))
            .Add(new DestroyGameEntityHandleSystem(contexts))
            .Add(new DestroyManageEntityHandleSystem(contexts))
            ;
    }

    private Systems CreateLogicSystems(Contexts contexts)
    {
        return new Feature("Logic")
            //Initialization
            .Add(new InitializePathSystem(contexts))
            .Add(new InitializePlayerSystem(contexts))
            

            //RayCasting
            .Add(new BallRayCastSystem(contexts))
            //Overlaping
            .Add(new BallOverlapSystem(contexts))

            //Counter
            .Add(new TickCountersSystem(contexts))

            //Animation finishing
            .Add(new FinishMoveAnimationSystem(contexts))


            //Collision actions
            .Add(new EnteringBallsToScreenSystem(contexts))
            .Add(new CollisionObjectDestroySystem(contexts))        //разобраться с этими коллизиями
            .Add(new ExplodeBallSystem(contexts))
            .Add(new ConnectChainsSystem(contexts))
            .Add(new CollidingAndInsertingProjectileSystem(contexts))

            //Destroying balls in chain
            .Add(new MatchInsertedBallInChainSystem(contexts))
            .Add(new VisualDestroyingBallsSystem(contexts))
            .Add(new CutChainSystem(contexts))

            //Spawn
            .Add(new CheckAndSpawnBallSystem(contexts))

            //Movement
            .Add(new UpdateBallDistanceBySpeedSystem(contexts))
            .Add(new ChangeBallPositionOnPathSystem(contexts))

            //Updating
            .Add(new SetChainEdgesSystem(contexts))
            .Add(new SetChainSpeedSystem(contexts))

            //Animation beginning
            .Add(new MoveAnimationControlSystem(contexts))
            .Add(new ScaleAnimationControlSystem(contexts))


            //Colors
            .Add(new UpdateColorBallSystem(contexts))
            .Add(new CountBallColorsSystem(contexts))


            //Score
            .Add(new ScoreCounterSystem(contexts))


            //Ability
            .Add(new InputAbilitySystem(contexts))       // this system just for test and imitating ability invoking behaviour
            .Add(new InvokingAbilitySystem(contexts))


            //Input
            .Add(new TouchHandleSystem(contexts))
            //Player
            .Add(new RotatePlayerSystem(contexts))
            .Add(new ShootPlayerSystem(contexts))
            .Add(new BallExchangePlayerSystem(contexts))
            .Add(new UpdatePointerLengthSystem(contexts))
            //Shooting
            .Add(new ShootingForceSystem(contexts))

            //GameOver
            .Add(new GameEndProcessSystem(contexts))
            ;
    }

#region Logger Methods
    private void InitializeLogger()
    {
        try
        {
            if (LogManager.Configuration != null)
                return;

            var target1 = new NLog.Targets.FileTarget();
            target1.FileName = Path.Combine(GetTempFolder(), "Debug.log");
            target1.KeepFileOpen = false;
            target1.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=ToString}";
            var target2 = new NLog.Targets.DebuggerTarget();

            LogManager.Configuration = new NLog.Config.LoggingConfiguration();
            LogManager.Configuration.AddTarget("logFile", target1);
            LogManager.Configuration.AddTarget("debug", target2);
            LogManager.Configuration.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Trace, target1));
            LogManager.Configuration.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Trace, target2));
            LogManager.ReconfigExistingLoggers();
        }
        catch (Exception)
        {
        }
        finally
        {
            logger = LogManager.GetCurrentClassLogger();
        }
    }

    private static string GetTempFolder()
    {
        if (string.IsNullOrEmpty(tempFolder))
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            tempFolder = Path.Combine(appData, "NeonZuma");

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
        }

        return tempFolder;
    }
#endregion
}
