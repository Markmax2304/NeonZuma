﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Entitas;

public class TouchHandleSystem : IExecuteSystem
{
    private Contexts _contexts;

    bool isPressed = false;

    public TouchHandleSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        if (Input.GetMouseButtonDown(0)) {
            PointerEventData data;
            data = GetPointerData(-1);
            //data = GetPointerData(Input.touches[0].fingerId);

            if (data == null) {
                return;
            }

            InputEntity touchEntity = _contexts.input.CreateEntity();

            if (data.pointerEnter != null && data.pointerEnter.layer == 5) {
                touchEntity.AddTouchType(TypeTouch.Interact);
                Debug.Log("Interact");
                touchEntity.isDestroyed = true;
            }
            else {
                touchEntity.AddTouchType(TypeTouch.Rotate);
                touchEntity.AddTouchPosition(GetMousePosition());
            }
        }

        if (Input.GetMouseButton(0)) {
            InputEntity[] inputs = _contexts.input.GetEntities(InputMatcher.TouchPosition);

            for(int i = 0; i < inputs.Length; i++) {
                inputs[i].ReplaceTouchPosition(GetMousePosition());
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            InputEntity[] inputs = _contexts.input.GetEntities(InputMatcher.TouchPosition);

            for (int i = 0; i < inputs.Length; i++) {
                inputs[i].ReplaceTouchPosition(GetMousePosition());
                inputs[i].ReplaceTouchType(TypeTouch.Shoot);
                Debug.Log("Shoot");
                inputs[i].isDestroyed = true;
            }
        }
    }

    PointerEventData GetPointerData(int id)
    {
        StandaloneModule currentInput = EventSystem.current.currentInputModule as StandaloneModule;
        if (currentInput == null) {
            return null;
        }

        return currentInput.GetPointerEventData(id);

    }

    Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}