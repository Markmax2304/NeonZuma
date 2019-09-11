using UnityEngine;
using Entitas;
using PathCreation;

public class InitializePathSystem : IInitializeSystem
{
    private Contexts _contexts;

    public InitializePathSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        var paths = _contexts.global.levelConfig.value.pathCreatorPrefabs;

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
        }
    }
}
