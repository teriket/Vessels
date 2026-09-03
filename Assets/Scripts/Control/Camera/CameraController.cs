using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;
using Development;
using System;
using DataStructures;

public class CameraController : MonoBehaviour
{

    public enum CameraMode
    {
        ORBIT,
        FIRST_PERSON,
        TWO_D_FOLLOW,
        TWO_D_PUSH_AHEAD
    }

    [Flags]
    public enum CameraModifiers
    {
        NONE = 0,
        COLLISION = 1 << 0,
        TRANSPARENCY = 1 << 1,
    }

    [SerializeField] CameraMode cameraMode;
    [SerializeField] CameraModifiers cameraBehaviourModifiers;
    [SerializeField][Range(0, 1)] float followSpeed = 0.5f;
    private ICameraTransform optimisticCameraTarget;
    // TODO: ordered list of camera modifiers
    private Vector3 currentVelocity;

    void Start()
    {
        SetCameraMode(cameraMode);
    }

    public void SetCameraMode(CameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CameraMode.ORBIT:
                optimisticCameraTarget = new OrbitCamera();
                break;
            case CameraMode.FIRST_PERSON:
                optimisticCameraTarget = null;
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
        // TODO: check the optimistic camera positions target vs any modifiers, and adjust accordingly

        // smoothly move towards the position the cameraTarget specifies
        transform.position = Vector3.SmoothDamp(transform.position, optimisticCameraTarget.GetTargetPosition(), ref currentVelocity, followSpeed);

        // look towards the point in space the cameraTarget specifies
        transform.LookAt(optimisticCameraTarget.GetLookAtPosition());
    }

}
