using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class UpdateBallDistanceBySpeedSystem : IExecuteSystem
{
    private Contexts _contexts;
    private Dictionary<int, float> trackLengths;
    private float trackPercent;

    private bool isUpdated = false;

    public UpdateBallDistanceBySpeedSystem(Contexts contexts)
    {
        _contexts = contexts;
        trackPercent = _contexts.global.levelConfig.value.normalSpeedLengthPercent;
    }

    public void Execute()
    {
        if (_contexts.global.isFreeze)
            return;

        float delta = _contexts.global.deltaTime.value;
        var paths = _contexts.game.GetEntities(GameMatcher.TrackId);

        InitTrackLengths(paths);

        foreach(var path in paths)
        {
            var chains = path.GetChains(true);
            if(chains == null)
            {
                _contexts.manage.CreateEntity()
                    .AddLogMessage("Failed to update distance ball. Chain collection is null", TypeLogMessage.Error, true, GetType());
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

    #region Private Methods
    private void InitTrackLengths(GameEntity[] paths)
    {
        if (trackLengths == null)
        {
            trackLengths = new Dictionary<int, float>();
            foreach (var track in paths)
            {
                trackLengths.Add(track.trackId.value, track.pathCreator.value.path.length);
            }
        }
    }

    private void CheckDistanceToEnd(GameEntity path, GameEntity ball, float speed)
    {
        if (!isUpdated && speed > 0)
        {
            bool oldNearValue = path.isNearToEnd;

            if (ball.distanceBall.value >= trackLengths[path.trackId.value] * trackPercent)
                path.isNearToEnd = true;
            else
                path.isNearToEnd = false;

            if (oldNearValue != path.isNearToEnd)
                path.isUpdateSpeed = true;

            isUpdated = true;
        }
    }
    #endregion
}
