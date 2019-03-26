using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class CheckSpawnBallSystem : IExecuteSystem
{
    private Contexts _contexts;

    public CheckSpawnBallSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        GameEntity[] balls = _contexts.game.GetEntities(GameMatcher.LastBall);
            
        for(int i = 0; i < balls.Length; i++) {
            if (balls[i].hasPositionBall) {
                if(balls[i].positionBall.value >= _contexts.game.levelConfig.value.offsetBetweenBalls) { 
                    var paths = _contexts.game.GetEntitiesWithTrackId(balls[i].trackId.value);

                    foreach(var path in paths) {
                        if (path.isTrack) {
                            path.isTimeToSpawn = true;
                        }
                    }
                }
            }
        }
    }
}
