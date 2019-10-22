using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class TrackStorage
{
    #region Fields
    private Track[] trackEntities;
    private List<Chain> cachedChains;
    #endregion

    #region Constructors
    public TrackStorage(GameEntity[] tracks)
    {
        trackEntities = new Track[tracks.Length];
        for(int i = 0; i < tracks.Length; i++)
        {
            trackEntities[i] = new Track(tracks[i]);
        }

        cachedChains = new List<Chain>();
    }
    #endregion

    #region Public Methods
    public void AddBall(GameEntity ball)
    {
        for(int i = 0; i < trackEntities.Length; i++)
        {
            for(int j = 0; j < trackEntities[i].chainEntities.Count; j++)
            {
                if(trackEntities[i].chainEntities[j].chainEntity.chainId.value == ball.parentChainId.value &&
                    !HasBallAtStorage(ball, trackEntities[i].chainEntities[j]))
                {
                    InsertBall(trackEntities[i].chainEntities[j], ball);
                    return;
                }
            }
        }
    }

    public void AddChain(GameEntity chain)
    {
        for(int i = 0; i < trackEntities.Length; i++)
        {
            if(trackEntities[i].trackEntity.trackId.value == chain.parentTrackId.value &&
                !HasChainAtStorage(chain, trackEntities[i]))
            {
                InsertChain(trackEntities[i], chain);
                return;
            }
        }
    }

    public void RemoveBall(GameEntity ball, int chainId = -1)
    {
        if (chainId == -1)
            chainId = ball.parentChainId.value;

        for (int i = 0; i < trackEntities.Length; i++)
        {
            for (int j = 0; j < trackEntities[i].chainEntities.Count; j++)
            {
                if (trackEntities[i].chainEntities[j].chainEntity.chainId.value == chainId)
                {
                    trackEntities[i].chainEntities[j].ballEntities.Remove(ball);
                    return;
                }
            }
        }
    }

    public void RemoveChain(GameEntity chainEntity)
    {
        for (int i = 0; i < trackEntities.Length; i++)
        {
            if (trackEntities[i].trackEntity.trackId.value == chainEntity.parentTrackId.value)
            {
                for(int j = 0; j < trackEntities[i].chainEntities.Count; j++)
                {
                    if(trackEntities[i].chainEntities[j].chainEntity.chainId.value == chainEntity.chainId.value)
                    {
                        var chain = trackEntities[i].chainEntities[j];
                        chain.Deactivate();
                        trackEntities[i].chainEntities.Remove(chain);
                        return;
                    }
                }
            }
        }
    }

    public Track[] GetTracks()
    {
        return trackEntities;
    }

    public List<Chain> GetChains(int parentTrackId, bool sorted = false)
    {
        for(int i = 0; i < trackEntities.Length; i++)
        {
            if (trackEntities[i].trackEntity.trackId.value == parentTrackId)
            {
                if (sorted)
                    trackEntities[i].chainEntities.Sort();

                return trackEntities[i].chainEntities;
            }
        }

        return null;
    }

    public List<GameEntity> GetBalls(int parentChainId)
    {
        for(int i = 0; i < trackEntities.Length; i++)
        {
            for(int j = 0; j < trackEntities[i].chainEntities.Count; j++)
            {
                if (trackEntities[i].chainEntities[j].chainEntity.chainId.value == parentChainId)
                    return trackEntities[i].chainEntities[j].ballEntities;
            }
        }

        return null;
    }
    #endregion

    #region Private Mehtods
    /// <summary>
    /// Sorted inserting ball into chain
    /// </summary>
    private void InsertBall(Chain chain, GameEntity ballEntity)
    {
        var balls = chain.ballEntities;
        int count = balls.Count;

        // first place
        if (balls.Count == 0 || ballEntity.distanceBall.value < balls[count - 1].distanceBall.value)
        {
            balls.Add(ballEntity);
            return;
        }

        // last place
        if (ballEntity.distanceBall.value > balls[0].distanceBall.value)
        {
            balls.Insert(0, ballEntity);
            return;
        }

        // somewhere in chain
        for (int i = 1; i < count; i++)
        {
            if (ballEntity.distanceBall.value > balls[i].distanceBall.value)
            {
                balls.Insert(i, ballEntity);
                return;
            }
        }
    }

    /// <summary>
    /// Unsorted inserting chain into track, another words it's attaching chain to track
    /// </summary>
    private void InsertChain(Track track, GameEntity chainEntity)
    {
        var chain = GetChainInstance();
        chain.Activate(chainEntity);

        track.chainEntities.Add(chain);
    }

    /// <summary>
    /// Return chain instance if there is non active, otherwise creates a new instance and return it
    /// </summary>
    private Chain GetChainInstance()
    {
        for(int i = 0; i < cachedChains.Count; i++)
        {
            if (!cachedChains[i].isActive)
                return cachedChains[i];
        }

        Chain chain = new Chain();
        cachedChains.Add(chain);
        return chain;
    }

    private bool HasChainAtStorage(GameEntity chainEntity, Track track)
    {
        for(int i = 0; i < track.chainEntities.Count; i++)
        {
            if (chainEntity.chainId.value == track.chainEntities[i].chainEntity.chainId.value)
                return true;
        }

        return false;
    }

    private bool HasBallAtStorage(GameEntity ballEntity, Chain chain)
    {
        for(int i = 0; i < chain.ballEntities.Count; i++)
        {
            if (ballEntity.ballId.value == chain.ballEntities[i].ballId.value)
                return true;
        }

        return false;
    }
    #endregion
}

#region Nested Types
public class Track
{
    public GameEntity trackEntity;
    /// <summary>
    /// Unsorted collection of chains
    /// </summary>
    public List<Chain> chainEntities;

    public Track(GameEntity track)
    {
        trackEntity = track;
        chainEntities = new List<Chain>();
    }
}

public class Chain : IComparable<Chain>
{
    public bool isActive;
    public GameEntity chainEntity;
    /// <summary>
    /// Sorted collection of balls
    /// </summary>
    public List<GameEntity> ballEntities;
    
    /// <summary>
    /// Distance on track by firs ball in this chain
    /// </summary>
    public float DistanceChain
    {
        get
        {
            return ballEntities.Count > 0 ? ballEntities[0].distanceBall.value : 1f;
        }
    }

    public Chain()
    {
        ballEntities = new List<GameEntity>();
        isActive = false;
    }

    public void Activate(GameEntity chain)
    {
        chainEntity = chain;
        isActive = true;
    }

    public void Deactivate()
    {
        chainEntity = null;
        isActive = false;
        ballEntities.Clear();
    }

    public int CompareTo(Chain other)
    {
        return other.DistanceChain > DistanceChain ? 1 : other.DistanceChain < DistanceChain ? -1 : 0;
    }
}
#endregion
