using UnityEngine;

namespace Control.Camera
{
    public interface ICameraEffect
    {
        public void Initialize();
        public bool ShouldTrigger(Vector3 targetPosition, Vector3 lookatPosition);
        public void ActivateEffect(Vector3 targetPosition, Vector3 lookAtPosition);
    }
}