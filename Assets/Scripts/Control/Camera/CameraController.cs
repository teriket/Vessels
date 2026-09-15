using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;
using Development;
using System;
using DataStructures;

namespace Control.Camera
{
    public class CameraController : MonoBehaviour
    {

        public enum CameraMode
        {
            ORBIT,
            // SHOULDER
            // FIRST_PERSON,
            // CINEMATIC_CONTROL,
            // TWO_D_FOLLOW,
            // TWO_D_PUSH_AHEAD
        }

        [Flags]
        public enum CameraModifiers
        {
            NONE = 0,
            SNAP_BETWEEN_ENVIRONMENT_AND_PLAYER = 1 << 0,
            PLAYER_TRANSPARENCY = 1 << 1,
            ENVIRONMENTAL_TRANSPARENCY = 1 << 2
            // SHY_CAMERA = 1 << 3,
        }

        // setting toggles
        [SerializeField] CameraMode cameraMode;
        [SerializeField] CameraModifiers cameraBehaviourModifiers;

        // lerp settings
        [SerializeField][Range(0, 1)] float followSpeed = 0.5f;
        private Vector3 currentVelocity;

        // modifier lists
        private List<ICameraTransformModifier> transformModifiers = new();
        private List<ICameraEffect> cameraEffects = new();

        // transform modifiers
        [SerializeField] OrbitCamera orbitCamera = new();
        [SerializeField] ICameraTransformModifier snapModifier = new CameraCollisionSnap();
        [SerializeField] ICameraTransformModifier shyCamera = new ShyCamera();

        // camera effects
        [SerializeField] PlayerTransparency playerTransparencyEffect = new();

        private ICameraTransform optimisticCameraTarget;


        void Start()
        {
            SetCameraMode(cameraMode);
            AddTransformModifiers();
            AddCameraEffects();
        }

        private void AddTransformModifiers()
        {
            if (cameraBehaviourModifiers.HasFlag(CameraModifiers.SNAP_BETWEEN_ENVIRONMENT_AND_PLAYER))
            {
                snapModifier.Initialize();
                transformModifiers.Add(snapModifier);
            }

            // if (cameraBehaviourModifiers.HasFlag(CameraModifiers.SHY_CAMERA))
            // {
            //     shyCamera.Initialize();
            //     transformModifiers.Add(shyCamera);
            // }
        }

        private void AddCameraEffects()
        {
            if (cameraBehaviourModifiers.HasFlag(CameraModifiers.PLAYER_TRANSPARENCY))
            {
                playerTransparencyEffect.Initialize();
                cameraEffects.Add(playerTransparencyEffect);
            }
        }

        public void SetCameraMode(CameraMode cameraMode)
        {
            switch (cameraMode)
            {
                case CameraMode.ORBIT:
                    orbitCamera.Initialize();
                    optimisticCameraTarget = orbitCamera;
                    break;
                default:
                    optimisticCameraTarget = new OrbitCamera();
                    break;
            }
        }

        void LateUpdate()
        {
            if (optimisticCameraTarget == null)
            {
                Development.Logger.CriticalMessage("This component requires an ICameraTransform to calculate the cameras target position and rotation, but a camera mode is not configured");
                return;
            }

            Vector3 target = optimisticCameraTarget.GetTargetPosition();
            Vector3 rotation = optimisticCameraTarget.GetLookAtPosition();

            // modify the cameras position as necessary
            foreach (ICameraTransformModifier modifier in transformModifiers)
            {
                target = modifier.ModifyPosition(target, rotation);
            }

            // apply all active camera effects
            foreach (ICameraEffect effect in cameraEffects)
            {
                if (effect.ShouldTrigger(target, rotation))
                {
                    effect.ActivateEffect(target, rotation);
                }
            }


            // smoothly move towards the position the cameraTarget specifies
            transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, followSpeed);

            // look towards the point in space the cameraTarget specifies
            transform.LookAt(optimisticCameraTarget.GetLookAtPosition());
        }

    }
}