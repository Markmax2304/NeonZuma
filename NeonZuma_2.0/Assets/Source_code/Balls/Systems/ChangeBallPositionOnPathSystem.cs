using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using PathCreation;

public class ChangeBallPositionOnPathSystem : ReactiveSystem<GameEntity>, ITearDownSystem
{
    private Contexts _contexts;
    private Dictionary<int, GameEntity> chains;
    private Dictionary<int, GameEntity> tracks;

    public ChangeBallPositionOnPathSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        chains = new Dictionary<int, GameEntity>();
        tracks = new Dictionary<int, GameEntity>();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        int count = entities.Count;

        for(int i = 0; i < count; i++)
        {
            var chain = GetChain(entities[i].parentChainId.value);
            var track = GetTrack(chain.parentTrackId.value);

            if (track == null)
            {
                Debug.LogError($"Path with trackId - {chain.parentTrackId.value} doesn't exist.");
            }

            float distance = entities[i].distanceBall.value;
            PathCreator pathCreator = track.pathCreator.value;

            // Move Position
            Vector2 position = pathCreator.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop);
            // TODO: change to MovePosition()
            entities[i].transform.value.position = position;

            // Rotate
            // increase CPU perfomance by 150 %     // try to optimize
            Vector3 direction = pathCreator.path.GetDirectionAtDistance(distance, EndOfPathInstruction.Stop);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.down, direction);
            entities[i].transform.value.rotation = rotation;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasDistanceBall && entity.hasTransform;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.DistanceBall);
    }

    public void TearDown()
    {
        chains.Clear();
        tracks.Clear();
    }

    #region Private Methods
    private GameEntity GetChain(int chainId)
    {
        if (!chains.ContainsKey(chainId))
        {
            var newChain = _contexts.game.GetEntitiesWithChainId(chainId).SingleEntity();
            chains.Add(chainId, newChain);
        }

        return chains[chainId];
    }

    private GameEntity GetTrack(int trackId)
    {
        if (!tracks.ContainsKey(trackId))
        {
            var newTrack = _contexts.game.GetEntitiesWithTrackId(trackId).SingleEntity();
            tracks.Add(trackId, newTrack);
        }

        return tracks[trackId];
    }
    #endregion
}
