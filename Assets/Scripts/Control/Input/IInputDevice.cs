using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputDevice
{
    Vector2 GetMouseDelta();
    Vector2 GetMousePosition();
    float GetScroll();
}
