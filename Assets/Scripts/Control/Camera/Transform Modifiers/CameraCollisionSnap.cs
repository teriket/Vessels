
using System;
using DataStructures.IOC;
using UnityEngine;
using Config;

namespace Control.Camera
{
    public class CameraCollisionSnap : ICameraTransformModifier, IIoCComponent<CameraCollisionSnap>
    {
        // being left commented out for now.  The layer mask would limit what gameobjects the camera could collide with.
        // LayerMask layerMask;

        public IoCContainer iocContainer { get; set; }
        private IIoCComponent<CameraCollisionSnap> ioc => this;

        Transform playerTransform;
        GameObject player;

        public void Initialize()
        {
            // masks that filters what colliders should be considered when casting a ray
            // currently disabled because it's not a problem, but would need to update line
            // if (Physics.LineCast(startPosition, endPosition, out hit, layerMask))
            // layerMask = LayerMask.GetMask();
            iocContainer = IoCContainer.GetInstance();

            if (ioc != null)
            {
                ioc.RequestGameObject(Strings.IOC_PLAYER, out player);
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }
        }

        public Vector3 ModifyPosition(Vector3 targetPosition, Vector3 lookatPosition)
        {
            // try to repair null player references
            if (player == null && !ioc.TryRepairGameObject(Strings.IOC_PLAYER, out player))
            {
                Development.Logger.Warn("Snap camera could not find an instance of the player");
                return targetPosition;
            }

            // try to repair null player transform reference
            if (playerTransform == null)
            {
                playerTransform = player.transform;
                if (playerTransform == null)
                {
                    Development.Logger.Warn("Tried to repair the reference to the player transform, but failed");
                    return targetPosition;
                }
            }

            Vector3 finalPosition;

            RaycastHit hit;
            if (Physics.Linecast(playerTransform.position, targetPosition, out hit))
            {
                finalPosition = hit.point;
            }
            else
            {
                finalPosition = targetPosition;
            }

            return finalPosition;
        }
    }
}