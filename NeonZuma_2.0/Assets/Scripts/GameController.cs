using System.Collections.Generic;

using NLog;
using UnityEngine;
using Entitas;
using System;
using System.IO;

public class GameController : MonoBehaviour
{
    public LevelConfig config;

    private Systems _systems;

    private static string tempFolder;
    private NLog.Logger logger;

    public static bool HasRecordToLog { get; set; } = true;

    void Start()
    {
        InitializeLogger();
        logger.Trace("Initializing Game Controller");

        Contexts contexts = Contexts.sharedInstance;

        contexts.game.SetLevelConfig(config);
        contexts.game.SetBallColors(new Dictionary<ColorBall, int>());

        _systems = CreateSystems(contexts);

        _systems.Initialize();
    }

    void Update()
    {
        try
        {
            if (HasRecordToLog)
            {
                logger.Trace("\n\n\n\t\t *** START NEW FRAME *** \n\n");
                HasRecordToLog = false;
            }

            _systems.Execute();
            _systems.Cleanup();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to process updarte frame");
        }
    }

    private void OnDestroy()
    {
        _systems.TearDown();
    }

    private Systems CreateSystems(Contexts contexts)
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
            //Inserting
            .Add(new CollidingAndInsertingProjectileSystem(contexts))

            // TODO: implement Queue of collision event here
            
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
}
