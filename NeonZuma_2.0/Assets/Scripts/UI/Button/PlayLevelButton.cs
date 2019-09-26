using UnityEngine;

public class PlayLevelButton : MonoBehaviour, IStartLevelListener, IFinishLevelListener
{
    private Contexts _contexts;
    private GameObject go;

    private void Awake()
    {
        go = gameObject;

        _contexts = Contexts.sharedInstance;
        var entity = _contexts.manage.CreateEntity();
        entity.AddStartLevelListener(this);
        entity.AddFinishLevelListener(this);
    }

    public void ButtonPressed()
    {
        _contexts.manage.isStartLevel = true;
    }

    #region Interface Methods
    public void OnStartLevel(ManageEntity entity)
    {
        go.SetActive(false);
    }

    public void OnFinishLevel(ManageEntity entity)
    {
        go.SetActive(true);
    }
    #endregion
}
