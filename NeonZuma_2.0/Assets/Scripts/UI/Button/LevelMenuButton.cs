using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuButton : MonoBehaviour, IStartLevelListener, IFinishLevelListener
{
    private Contexts _contexts;
    private GameObject go;

    private void Awake()
    {
        go = gameObject;
        go.SetActive(false);

        _contexts = Contexts.sharedInstance;
        var entity = _contexts.manage.CreateEntity();
        entity.AddStartLevelListener(this);
        entity.AddFinishLevelListener(this);
    }

    public void ButtonPressed()
    {
        _contexts.manage.isFinishLevel = true;
    }

    #region Interface Methods
    public void OnStartLevel(ManageEntity entity)
    {
        go.SetActive(true);
    }

    public void OnFinishLevel(ManageEntity entity)
    {
        go.SetActive(false);
    }
    #endregion
}
