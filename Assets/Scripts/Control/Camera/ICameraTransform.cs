using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraTransform
{
    Vector3 GetTargetPosition();
    Vector3 GetLookAtPosition();
}
