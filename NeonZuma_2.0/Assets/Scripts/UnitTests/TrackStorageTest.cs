using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class TrackStorageTest
{
    static int id = 0;

    public static void Test()
    {
        // set tracks
        var track = Contexts.sharedInstance.game.CreateEntity();
        track.AddTrackId(0);
        GameEntity[] tracks = new GameEntity[] { track };

        TrackStorage storage = new TrackStorage(tracks);

        Initialize(storage);

        InsertProjectiles(storage);
        
        CutChain(storage);

        GettingSortedChains(storage);
    }

    #region Private Methods
    private static void Initialize(TrackStorage storage)
    {
        // set chains
        for (int i = 0; i < 3; i++)
        {
            var chain = Contexts.sharedInstance.game.CreateEntity();
            chain.AddChainId(i);
            chain.AddParentTrackId(0);
            storage.AddChain(chain);
        }

        // wrong track id
        var badChain = Contexts.sharedInstance.game.CreateEntity();
        badChain.AddChainId(100);
        badChain.AddParentTrackId(5);
        storage.AddChain(badChain);

        // repeat chain id
        badChain = Contexts.sharedInstance.game.CreateEntity();
        badChain.AddChainId(0);
        badChain.AddParentTrackId(0);
        storage.AddChain(badChain);

        // set balls
        for (int i = 0; i < 3; i++)
        {
            int count = (int)Mathf.Pow(i + 1, 2);
            for (int j = 0; j < count; j++)
            {
                var ball = Contexts.sharedInstance.game.CreateEntity();
                ball.AddBallId(id++);
                ball.AddParentChainId(i);
                ball.AddDistanceBall(1 - id * 0.01f);
                storage.AddBall(ball);
            }
        }

        // wrong chain id
        var badBall = Contexts.sharedInstance.game.CreateEntity();
        badBall.AddParentChainId(5);
        badBall.AddBallId(id++);
        badBall.AddDistanceBall(1 - id * 0.01f);
        storage.AddBall(badBall);

        // repeat ball id
        badBall = Contexts.sharedInstance.game.CreateEntity();
        badBall.AddParentChainId(0);
        badBall.AddBallId(0);
        badBall.AddDistanceBall(1 - 0.01f);
        storage.AddBall(badBall);
    }

    private static void InsertProjectiles(TrackStorage storage)
    {
        // forward
        var projectile = Contexts.sharedInstance.game.CreateEntity();
        projectile.AddBallId(id++);
        projectile.AddParentChainId(1);
        projectile.AddDistanceBall(1f);
        storage.AddBall(projectile);

        // middle
        var p = Contexts.sharedInstance.game.CreateEntity();
        p.AddBallId(id++);
        p.AddParentChainId(1);
        p.AddDistanceBall(0.975f);
        storage.AddBall(p);

        // backward
        projectile = Contexts.sharedInstance.game.CreateEntity();
        projectile.AddBallId(id++);
        projectile.AddParentChainId(1);
        projectile.AddDistanceBall(0.9f);
        storage.AddBall(projectile);

        storage.RemoveBall(p);
    }

    private static void CutChain(TrackStorage storage)
    {
        var chainEntity = storage.GetChains(0)[1].chainEntity;
        var balls = storage.GetChains(0)[1].ballEntities;

        // *** NOT OPTIMIZED ***

        //// add two new chains firstly
        //var newChain = Contexts.sharedInstance.game.CreateEntity();
        //newChain.AddChainId(3);
        //newChain.AddParentTrackId(0);
        //storage.AddChain(newChain);

        //newChain = Contexts.sharedInstance.game.CreateEntity();
        //newChain.AddChainId(4);
        //newChain.AddParentTrackId(0);
        //storage.AddChain(newChain);

        //// readd balls half of balls per one new chain
        //for(int i = 0; i < balls.Count; i++)
        //{
        //    if(i < balls.Count / 2)
        //        balls[i].ReplaceParentChainId(3);
        //    else
        //        balls[i].ReplaceParentChainId(4);

        //    storage.AddBall(balls[i]);
        //}

        //// remove chain only lastly, after reattaching balls
        //storage.RemoveChain(chainEntity);

        // *** BETTER WAY ***
        var newChain = Contexts.sharedInstance.game.CreateEntity();
        newChain.AddChainId(3);
        newChain.AddParentTrackId(0);
        storage.AddChain(newChain);

        List<GameEntity> removedBalls = new List<GameEntity>();

        for(int i = (int)(balls.Count / 2f); i < balls.Count; i++)
        {
            // TODO: implement inside mechanism for transfering balls from one chain to another
            var ball = balls[i];
            ball.ReplaceParentChainId(3);
            storage.AddBall(ball);
            removedBalls.Add(ball);
        }

        foreach(var ball in removedBalls)
        {
            storage.RemoveBall(ball, 1);
        }
    }

    private static void GettingSortedChains(TrackStorage storage)
    {
        var chains = storage.GetChains(0, true);
    }
    #endregion
}
