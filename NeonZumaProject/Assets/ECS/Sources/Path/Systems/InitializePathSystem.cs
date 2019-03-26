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
        for(int i = 0; i < paths.Length; i++) {
            GameObject path = GameObject.Instantiate(paths[i], Vector3.zero, Quaternion.identity);

            GameEntity entity = _contexts.game.CreateEntity();
            entity.AddPathCreator(path.GetComponent<PathCreator>());
            entity.AddTrackId(i);
            entity.isTrack = true;
            entity.isTimeToSpawn = true;
        }
    }
}
