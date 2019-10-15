using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Entitas;

public class TrackStorage
{
    #region Fields
    private Dictionary<int, Track> trackEntities;
    #endregion

    #region Constructors
    public TrackStorage(GameEntity[] tracks)
    {
        trackEntities = new Dictionary<int, Track>(tracks.Length);
        for(int i = 0; i < tracks.Length; i++)
        {
            trackEntities[tracks[i].trackId.value] = new Track(tracks[i]);
        }
    }
    #endregion

    #region Public Methods
    public void AddBall(GameEntity ball)
    {

    }
    #endregion

    #region Nested Types
    struct Track
    {
        public GameEntity trackEntity;

        private Dictionary<int, Chain> chainEntities;

        public Track(GameEntity track)
        {
            trackEntity = track;
            chainEntities = new Dictionary<int, Chain>();
        }
    }

    struct Chain
    {
        public GameEntity chainEntity;

        private Dictionary<int, GameEntity> ballEntities;

        public Chain(GameEntity chain)
        {
            chainEntity = chain;
            ballEntities = new Dictionary<int, GameEntity>();
        }
    }
    #endregion
}
