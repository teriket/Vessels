using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;
using Development;
using DataStructures;

namespace Control
{
    public class OrbitCamera : ICameraTransform, IIoCComponent<ICameraTransform>
    {
        // a reference to the actual iocContainer that the helper interface requires
        public IoCContainer iocContainer { get; set; }

        // faster access to helper methods from the IoCComponent interface.  Also includes wrapper
        // functions for the IoCContainer itself with built-in safety checks, so IoC operations should
        // be defaulted to this variable instead of the iocContainer variable.
        private IIoCComponent<ICameraTransform> ioc => this;

        private GameObject player;
        private IMouseManager mouseManager;

        private const string PLAYER_IOC_GAMEOBJECT_NAME = "PlayerModel";

        private float phi;
        private float theta;
        private const float CAMERA_LOCK_PROTECTION_FACTOR = 0.1f;

        [SerializeField] float panSpeed = 1f;
        [SerializeField] float zoomSensitivity = 2;

        [SerializeField] private float minArmLength = 0;
        [SerializeField] private float maxArmLength = 10;
        [SerializeField] private float armLength = 10;

        [SerializeField] private float minPhi = -Mathf.PI + CAMERA_LOCK_PROTECTION_FACTOR;
        [SerializeField] private float maxPhi = 0 - CAMERA_LOCK_PROTECTION_FACTOR;

        public OrbitCamera()
        {
            ioc.InitializeIoCContainer(this);

            mouseManager = iocContainer.RequestComponent<IMouseManager>();
            player = iocContainer.RequestObject(PLAYER_IOC_GAMEOBJECT_NAME);

            if (mouseManager == null)
            {
                Development.Logger.Warn("Could not find a mouse manager to update cameras target position at object startup");
            }

            if (player == null)
            {
                Development.Logger.Warn("Could not find a player object to follow at object startup");
            }
        }

        /// <summary>
        /// Calculates the target position to move a cameras position to.  This uses spherical coordinates with a radius
        /// from the armLength, and position on the surface calculated by the mouse x,y delta from the initial frame of the game
        /// </summary>
        /// <returns>A vector3 with x,y,z coordinates</returns>
        public Vector3 GetTargetPosition()
        {
            // null reference checks has built-in attempt to regain the object reference and logging if it fails
            if (player == null && !ioc.TryRepairGameObject(PLAYER_IOC_GAMEOBJECT_NAME, out player))
            {
                return new Vector3();
            }

            if (mouseManager == null && !ioc.TryRepairComponentReference<IMouseManager>(out mouseManager))
            {
                return new Vector3();
            }

            // calculates the target position as a local offset on a sphere with radius "armLength" about the player transform
            Vector3 playerPosition = player.transform.position;
            Vector3 localSphericalPosition = CalculateSphericalCoordinates();
            Vector3 targetPosition = playerPosition + localSphericalPosition;

            return targetPosition;
        }

        /// <summary>
        /// Calculates the vector3 coordinate the camers should look at.  In this case, it's the player model
        /// </summary>
        /// <returns>The coordinates of the player model</returns>
        public Vector3 GetLookAtPosition()
        {
            // null reference checks has built-in attempt to regain the object reference and logging if it fails
            if (player == null && !ioc.TryRepairGameObject(PLAYER_IOC_GAMEOBJECT_NAME, out player))
            {
                return new Vector3();
            }

            return player.transform.position;
        }

        /// <summary>
        /// Calculates the cartesian coordinates of a point on a sphere with radius "armLength" with an origin 0,0,0.
        /// The radius can be modified with the mouse scroll wheel and the position modified with mouse deltas.
        /// </summary>
        /// <returns>A Vector3 coordinate of a point on a sphere with radius armLength.</returns>
        private Vector3 CalculateSphericalCoordinates()
        {
            const int INVERTED_CONTROLS_CONVERSION_FACTOR = -1;

            // calculates the angle of the camera about the sphere
            theta = mouseManager.GetMouseDelta().x * panSpeed / Mathf.Rad2Deg * INVERTED_CONTROLS_CONVERSION_FACTOR;
            phi = mouseManager.GetMouseDelta().y * panSpeed / Mathf.Rad2Deg;
            phi = Mathf.Clamp(phi, minPhi, maxPhi);

            // set the radial distance the camera is from the spheres origin
            armLength = armLength + (mouseManager.GetScroll() * zoomSensitivity * INVERTED_CONTROLS_CONVERSION_FACTOR);
            armLength = Mathf.Clamp(armLength, minArmLength, maxArmLength);

            // spherical coordinates -> cartesian coordinates
            Vector3 position = new Vector3(
                Mathf.Sin(theta) * Mathf.Sin(phi),
                Mathf.Cos(phi),
                Mathf.Cos(theta) * Mathf.Sin(phi)
            );

            return armLength * position;
        }

        void OnDestroy()
        {
            ioc.ExecuteCleanup();
        }

    }
}