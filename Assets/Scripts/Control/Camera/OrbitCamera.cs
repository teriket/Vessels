using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;
using Development;
using UnityEngine.SocialPlatforms;

public class OrbitCamera : ICameraTransform
{
    private GameObject player;
    private IMouseManager mouseManager;

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
        PlayerManager playerManager = PlayerManager.GetInstance();
        if (playerManager == null)
        {
            Development.Logger.CriticalMessage("Could not get a reference to the player manager");
            return;
        }


        mouseManager = playerManager.RequestComponent<IMouseManager>();
        player = playerManager.RequestObject("Player");

        if (mouseManager == null)
        {
            Development.Logger.Warn("Could not find a mouse manager to update cameras target position");
        }

        if (player == null)
        {
            Development.Logger.Warn("Could not find a player object to follow");
        }
    }

    public Vector3 GetTargetPosition()
    {
        if (player == null)
        {
            Development.Logger.Warn("Could not find a player to follow");
            return new Vector3();
        }

        if (mouseManager == null)
        {
            Development.Logger.Warn("Could not find a mouse manager to update rotations");
            return new Vector3();
        }

        Vector3 playerPosition = player.transform.position;
        Vector3 localSphericalPosition = CalculateSphericalCoordinates();
        Vector3 targetPosition = playerPosition + localSphericalPosition;

        return targetPosition;
    }

    public Vector3 GetLookAtPosition()
    {
        if (player == null)
        {
            Development.Logger.Warn("Could not find a player to look at");
            return new Vector3();
        }

        return player.transform.position;
    }

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
}
