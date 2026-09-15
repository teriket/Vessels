using UnityEngine;

namespace Control.Camera
{
    /// <summary>
    /// Recalculates the cameras target position if it would collide with a gameobject
    /// 
    /// </summary>
    public class ShyCamera : ICameraTransformModifier
    {

        // [SerializeField]
        // [Range(0, Mathf.Infinity)]
        // [Tooltip("How closely an object needs to be to the camera for it to nudge away from the object")]
        // float radius = 0.1f;

        // Collider[] hits = new Collider[5];

        public void Initialize()
        {

        }

        public Vector3 ModifyPosition(Vector3 targetPosition, Vector3 lookatPosition)
        {
            Vector3 target = targetPosition;

            // Physics.OverlapSphereNonAlloc(targetPosition, radius, hits);
            // foreach (var hitCollider in hits)
            // {
            //     if (hitCollider == null)
            //     {
            //         break;
            //     }

            //     Vector3 contactPoint = hitCollider.ClosestPoint(targetPosition);
            //     float correctiveDistance = radius - Vector3.Distance(contactPoint, targetPosition);

            //     Vector3 direction = (targetPosition - contactPoint).normalized;
            //     Vector3 rayEnd = direction * correctiveDistance;

            //     target += rayEnd;
            // }

            return target;
        }
    }
}