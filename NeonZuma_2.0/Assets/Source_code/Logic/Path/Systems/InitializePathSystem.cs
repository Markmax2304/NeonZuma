using UnityEngine;
using Entitas;
using PathCreation;

/// <summary>
/// Логика начальной инициализции треков
/// </summary>
public class InitializePathSystem : IInitializeSystem, ITearDownSystem
{
    private Contexts _contexts;

    public InitializePathSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        var paths = _contexts.global.levelConfig.value.pathCreatorPrefabs;
        GameEntity[] tracks = new GameEntity[paths.Length];

        for(int i = 0; i < paths.Length; i++)
        {
            GameObject path = GameObject.Instantiate(paths[i], Vector3.zero, Quaternion.identity);

            GameEntity trackEntity = _contexts.game.CreateEntity();
            var pathCreator = path.GetComponent<PathCreator>();
            trackEntity.AddPathCreator(pathCreator);

            int minLength = _contexts.global.levelConfig.value.minLengthSeries;
            int maxLength = _contexts.global.levelConfig.value.maxLengthSeries;
            trackEntity.AddRandomizer(new Randomizer(minLength, maxLength));
            trackEntity.AddTrackId(i);

            int countBallsOnEntirePath = (int)(pathCreator.path.length / _contexts.global.levelConfig.value.ballDiametr);
            trackEntity.AddGroupSpawn(countBallsOnEntirePath);
            // this component is controlling start and further spawn balls
            trackEntity.isSpawnAccess = true;

            tracks[i] = trackEntity;
        }

        // init track storage
        _contexts.game.SetTrackStorage(new TrackStorage(tracks));
    }

    public void TearDown()
    {
        var paths = _contexts.game.GetEntities(GameMatcher.TrackId);
        foreach (var path in paths)
        {
            var pathCreator = path.pathCreator.value;
            GameObject.Destroy(pathCreator.gameObject);
            path.Destroy();
        }

        _contexts.game.RemoveTrackStorage();
    }
}
