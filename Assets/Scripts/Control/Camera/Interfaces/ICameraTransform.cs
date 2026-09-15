using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Control.Camera
{
    public interface ICameraTransform
    {
        public void Initialize();
        Vector3 GetTargetPosition();
        Vector3 GetLookAtPosition();
    }
}