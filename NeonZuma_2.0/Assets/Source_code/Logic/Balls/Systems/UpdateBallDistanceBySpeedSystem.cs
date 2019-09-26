using System.Collections.Generic;

using UnityEngine;
using Entitas;

/// <summary>
/// Логика обновления позиции шаров относительно скорости. То есть, своего рода, логика движения шаров по треку
/// </summary>
public class UpdateBallDistanceBySpeedSystem : IExecuteSystem, IInitializeSystem, ITearDownSystem
{
    private Contexts _contexts;
    private Dictionary<int, float> trackLengths;
    private float trackPercent;

    private bool isUpdated = false;

    public UpdateBallDistanceBySpeedSystem(Contexts contexts)
    {
        _contexts = contexts;
        trackLengths = new Dictionary<int, float>();
    }

    public void Initialize()
    {
        trackPercent = _contexts.global.levelConfig.value.normalSpeedLengthPercent;
    }

    public void Execute()
    {
        if (_contexts.global.isFreeze)
            return;

        float delta = _contexts.global.deltaTime.value;
        var tracks = _contexts.game.GetEntities(GameMatcher.TrackId);

        InitTrackLengths(tracks);

        foreach(var path in tracks)
        {
            var chains = path.GetChains(true);
            if(chains == null)
            {
                //_contexts.manage.CreateEntity()
                //    .AddLogMessage("___ Failed to update distance ball. Chain collection is null", TypeLogMessage.Error, true, GetType());
                continue;
            }

            for (int i = 0; i < chains.Count; i++)
            {
                if (chains[i] == null)
                    continue;

                float speed = chains[i].chainSpeed.value;
                if (speed == 0)
                    continue;

                var balls = chains[i].GetChainedBalls(true);
                if (balls == null)
                    continue;

                for (int j = 0; j < balls.Count; j++)
                {
                    if (balls[j].hasBallId)
                    {
                        float distance = balls[j].distanceBall.value;
                        balls[j].ReplaceDistanceBall(distance + delta * speed);
                    }
                }

                CheckDistanceToEnd(path, balls[0], speed);
            }

            isUpdated = false;
        }
    }

    public void TearDown()
    {
        trackLengths.Clear();
        _contexts.global.isBallReachedEnd = false;
    }

    #region Private Methods
    private void InitTrackLengths(GameEntity[] paths)
    {
        if (trackLengths.Count == 0)
        {
            trackLengths = new Dictionary<int, float>();
            foreach (var track in paths)
            {
                trackLengths.Add(track.trackId.value, track.pathCreator.value.path.length);
            }
        }
    }

    // Weakness method and its invoking is weakness too
    private void CheckDistanceToEnd(GameEntity path, GameEntity ball, float speed)
    {
        if (!isUpdated && speed > 0)
        {
            bool oldNearValue = path.isNearToEnd;
            float trackLength = trackLengths[path.trackId.value];

            if (ball.distanceBall.value >= trackLength)
            {
                _contexts.global.isBallReachedEnd = true;
                ball.AddGroupDestroy(Extensions.DestroyGroupId);
            }
            else
            {
                path.isNearToEnd = ball.distanceBall.value >= trackLength * trackPercent;
            }

            if (oldNearValue != path.isNearToEnd)
                path.isUpdateSpeed = true;

            isUpdated = true;
        }
    }
    #endregion
}
