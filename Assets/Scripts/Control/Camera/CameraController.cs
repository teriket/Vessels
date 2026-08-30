using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;
using Development;

public class CameraController : MonoBehaviour
{

    public enum CameraMode
    {
        ORBIT,
        FIRST_PERSON
    }

    [SerializeField] CameraMode cameraMode;
    [SerializeField][Range(0, 1)] float followSpeed = 0.5f;
    private ICameraTransform cameraTarget;

    void Start()
    {
        SetCameraMode(cameraMode);
    }

    public void SetCameraMode(CameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CameraMode.ORBIT:
                cameraTarget = new OrbitCamera();
                break;
            case CameraMode.FIRST_PERSON:
                cameraTarget = null;
                break;
            default:
                cameraTarget = new OrbitCamera();
                break;
        }
    }

    void LateUpdate()
    {
        if (cameraTarget == null)
        {
            Development.Logger.CriticalMessage("This component requires an ICameraTransform to calculate the cameras target position and rotation, but a camera mode is not configured");
            return;
        }

        // smoothly move towards the position the cameraTarget specifies
        transform.position = Vector3.Lerp(transform.position, cameraTarget.GetTargetPosition(), followSpeed);

        // look towards the point in space the cameraTarget specifies
        transform.LookAt(cameraTarget.GetLookAtPosition());
    }
}
