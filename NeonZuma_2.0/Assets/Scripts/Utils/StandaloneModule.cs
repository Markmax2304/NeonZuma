using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StandaloneModule : StandaloneInputModule
{
    public PointerEventData GetPointerEventData(int id)         //-1 - leftButton, -2 - rightButton, -3 - middleButton
    {
        return GetLastPointerEventData(id);
    }
}
