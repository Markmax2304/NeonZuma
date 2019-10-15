using UnityEngine;
using UnityEngine.EventSystems;
using Entitas;

/// <summary>
/// Логика отлова и инициализции тач ввода в игре
/// </summary>
public class TouchHandleSystem : IExecuteSystem, ITearDownSystem
{
    private Contexts _contexts;
    private Camera camera;

    private Vector2 GetMousePosition { get { return camera.ScreenToWorldPoint(Input.mousePosition); } }

    public TouchHandleSystem(Contexts contexts)
    {
        _contexts = contexts;
        camera = Camera.main;
    }

    public void Execute()
    {
        if (Input.GetMouseButtonDown(0)) {
            PointerEventData data;
#if UNITY_EDITOR
            data = GetPointerData(-1);
#elif UNITY_ANDROID
            data = GetPointerData(Input.touches[0].fingerId);
#endif

            if (data == null)
            {
                return;
            }

            InputEntity touchEntity = _contexts.input.CreateEntity();

            if (data.pointerEnter != null && data.pointerEnter.gameObject.CompareTag(Constants.PLAYER_TAG))
            {
                touchEntity.AddTouchType(TypeTouch.Exchange);
                touchEntity.isDestroyed = true;
            }
            else
            {
                touchEntity.AddTouchType(TypeTouch.Rotate);
                touchEntity.AddTouchPosition(GetMousePosition);
            }
        }

        if (Input.GetMouseButton(0))
        {
            InputEntity[] inputs = _contexts.input.GetEntities(InputMatcher.TouchPosition);

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i].ReplaceTouchPosition(GetMousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            InputEntity[] inputs = _contexts.input.GetEntities(InputMatcher.TouchPosition);

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i].ReplaceTouchPosition(GetMousePosition);
                inputs[i].ReplaceTouchType(TypeTouch.Shoot);
                inputs[i].isDestroyed = true;
            }
        }
    }

    public void TearDown()
    {
        var inputs = _contexts.input.GetEntities(InputMatcher.TouchType);
        foreach (var input in inputs)
        {
            input.Destroy();
        }
    }

    #region Private Methods
    private PointerEventData GetPointerData(int id)
    {
        StandaloneModule currentInput = EventSystem.current.currentInputModule as StandaloneModule;
        if (currentInput == null) {
            return null;
        }

        return currentInput.GetPointerEventData(id);

    }
    #endregion
}
