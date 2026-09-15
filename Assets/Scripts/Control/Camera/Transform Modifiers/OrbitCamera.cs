using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Development;
using DataStructures.IOC;
using DataStructures.Settings;
using UnityEngine.InputSystem;
using Control.Controller;
using Config;
using System;

namespace Control.Camera
{
    [Serializable]
    public class OrbitCamera : ICameraTransform, IIoCComponent<ICameraTransform>
    {
        // a reference to the actual iocContainer that the helper interface requires
        public IoCContainer iocContainer { get; set; }

        // faster access to helper methods from the IoCComponent interface.  Also includes wrapper
        // functions for the IoCContainer itself with built-in safety checks, so IoC operations should
        // be defaulted to this variable instead of the iocContainer variable.
        private IIoCComponent<ICameraTransform> ioc => this;

        private GameObject player;
        private ControlManager controlManager;
        private PlayerControls controller;

        private float phi;
        private float theta;

        [Header("Player Pref Bindings")]
        [SerializeField] private FloatBindingSO panSpeed;
        [SerializeField] private FloatBindingSO zoomSensitivity;
        [SerializeField] private BoolBindingAsIntSO flipCameraHorizontalAxis;
        [SerializeField] private BoolBindingAsIntSO flipCameraVerticalAxis;

        [Header("Camera Arm Management")]
        [SerializeField] private float minArmLength = 0;
        [SerializeField] private float maxArmLength = 10;
        [SerializeField] private float armLength = 10;

        [Header("Camera Panning")]
        [SerializeField] private float cameraLockProtectionFactor = 0.1f;
        [SerializeField] private float minPhi = -Mathf.PI;
        [SerializeField] private float maxPhi = 0;
        [SerializeField] private float defaultPhiOffset = 0;

        public void Initialize()
        {
            iocContainer = IoCContainer.GetInstance();

            if (panSpeed == null)
            {
                Development.Logger.CriticalMessage("A binding for the pan speed setting was not configured.  Did you remember to select the scriptable object in the editor?");
                return;
            }

            if (zoomSensitivity == null)
            {
                Development.Logger.CriticalMessage("A binding for the zoom sensitivity setting was not configured.  Did you remember to select the scriptable object in the editor?");
                return;
            }

            if (flipCameraHorizontalAxis == null)
            {
                Development.Logger.CriticalMessage("A binding for flipping the horizontal axis was not configured.  Did you remember to select the scriptable object in the editor?");
                return;
            }

            if (flipCameraVerticalAxis == null)
            {
                Development.Logger.CriticalMessage("A binding for flipping the vertical axis was not configured.  Did you remember to select the scriptable object in the editor?");
                return;
            }

            if (iocContainer != null)
            {
                controlManager = iocContainer.RequestComponent<ControlManager>();
                player = iocContainer.RequestObject(Strings.IOC_PLAYER);
            }
            else
            {
                return;
            }

            if (controlManager == null)
            {
                Development.Logger.Warn("Could not find a controller manager in the scene to accept player inputs from.");
            }
            else
            {
                controller = ControlManager.GetController();
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
            if (!InitializeComponents())
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
            if (player == null && !ioc.TryRepairGameObject(Strings.IOC_PLAYER, out player))
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
            const int INVERTED_SCROLL_VALUE_CONVERSION_FACTOR = -1;
            float scrollValue = controller.Gameplay.Scroll.ReadValue<float>() * INVERTED_SCROLL_VALUE_CONVERSION_FACTOR;

            // calculates the angle of the camera about the sphere
            float xDelta = controller.Gameplay.LookAction.ReadValue<Vector2>().x;
            float yDelta = controller.Gameplay.LookAction.ReadValue<Vector2>().y;

            theta += xDelta * panSpeed.Value * Time.deltaTime * flipCameraHorizontalAxis.Value;
            phi += (yDelta * panSpeed.Value * Time.deltaTime * flipCameraVerticalAxis.Value) - defaultPhiOffset;
            phi = Mathf.Clamp(phi, minPhi + cameraLockProtectionFactor, maxPhi - cameraLockProtectionFactor);

            // set the radial distance the camera is from the spheres origin
            armLength = armLength + (scrollValue * zoomSensitivity.Value);
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

        /// <summary>
        /// Check that no IOC-provided components are null and attempt to repair them
        /// </summary>
        /// <returns>False if a component is null and failed to be repaired</returns>
        bool InitializeComponents()
        {
            if (iocContainer == null && !ioc.TryRepairIoCContainer())
            {
                return false;
            }

            // null reference checks has built-in attempt to regain the object reference and logging if it fails
            if (player == null && !ioc.TryRepairGameObject(Strings.IOC_PLAYER, out player))
            {
                return false;
            }

            // reinitialize the controller manager
            if (controlManager == null && !ioc.TryRepairComponentReference<ControlManager>(out controlManager))
            {
                return false;
            }

            // fix controller references and reinit the initial mouse position if it wasn't cached
            if (controller == null)
            {
                controller = ControlManager.GetController();


                if (controller == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}