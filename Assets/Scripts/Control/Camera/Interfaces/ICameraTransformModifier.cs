using UnityEngine;

namespace Control.Camera
{
    public interface ICameraTransformModifier
    {
        public void Initialize();
        public Vector3 ModifyPosition(Vector3 targetPosition, Vector3 lookatPosition);
    }
}