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
        var paths = _contexts.game.levelConfig.value.pathCreatorPrefabs;

        for(int i = 0; i < paths.Length; i++)
        {
            GameObject path = GameObject.Instantiate(paths[i], Vector3.zero, Quaternion.identity);

            GameEntity trackEntity = _contexts.game.CreateEntity();
            trackEntity.AddPathCreator(path.GetComponent<PathCreator>());

            int minLength = _contexts.game.levelConfig.value.minLengthSeries;
            int maxLength = _contexts.game.levelConfig.value.maxLengthSeries;
            trackEntity.AddRandomizer(new Randomizer(minLength, maxLength));
            trackEntity.AddTrackId(i);
            trackEntity.isSpawnAccess = true;
            trackEntity.isTimeToSpawn = true;

            var chainEntity = _contexts.game.CreateEntity();
            chainEntity.AddChainId(Extensions.ChainId);
            chainEntity.AddParentTrackId(i);
            chainEntity.AddChainSpeed(_contexts.game.levelConfig.value.followSpeed);
        }
    }
}
