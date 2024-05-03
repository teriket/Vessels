using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
Author:         Tanner Hunt
Date:           5/2/2024
Version:        1.0.0
Description:    This Code handles camera movements and rotations around the player character.
ChangeLog:      V 0.1.0 -- 4/16/2024
                    --Added panning around the character in a sphere centered at character.
                    --Added Zooming controls with multiple animation types
                    --Dev time: 4 hours
                    --NYI: snapping the camera closer to the player if an object interferes with camera views
                V 0.1.1 -- 4/19/2024
                    --Moved pan() and lookat() to update() from fixedupdate() to fix character jitter
                    --altered panning so it happens with all mouse movements rather than clicks
                    --NYI: theta may need to be moved to it's own script, as it interacts with character movements, might interact with animation and spells later.
                    --NYI: Make mouse movements context sensitive so the camera doesn't move while a menu is open.
                    --Dev time: 0 Hours
                V 0.1.2 -- 4/20/2024
                    --Added then removed support for mouse context; it is now handled by
                    an event in the pausing script.
                    --Moved camera movements back to LateUpdate() because of new jitter
                    --Dev time: 0 Hours
                V 0.1.3 -- 4/22/2024
                    --Fixed an error where the camera could be zoomed in and out while the game
                    is paused
                    --Dev time: 0 hours
                V 0.1.4 -- 4/24/2024
                    --Fixed camera jitter again by removing the Time.deltaTime calculation
                    from camera panning
                    --Pausing of camera movement in pan is now done through mouse contexts
                    --Dev time: 0.25 hours
                V 1.0.0 -- 5/2/2024
                    --Camera now snaps closer to the player if an object is in the way
                    --Dev time -- 2 hours

*/
namespace Control{
public class CameraController : MonoBehaviour
{
    GameObject character;                           //The object to rotate around and zoom towards
    private IEnumerator coroutine;

    [Header("Pan Controls")]
    [SerializeField]float panSpeed = 1;             //How fast the camera should rotate around the character
    private float phi = 0.3f;                               //the angle of incident along the y axis of the cameras pan
    private float theta;                                    //the rotation of the camera along the x-z plane to the player character

    [Header("Zooming Controls")]
    private float armLength;                                //the current radius the camera has to the player
    private float snapToArmLength;                          //Where the camera should snap to, should an object interfere between the player and camera
    const float zoomStopThreshold = 0.01f;          //how close the camera can zoom to min zoom or max zoom to prevent locking of camera rotation
    [SerializeField]float zoomDistance;             //how far the camera should zoom in one scroll;  Updates the arm length
    [SerializeField]int numberOfZoomingFrames = 10; //the number of frames the zooming animation should take
    [SerializeField]float minZoom = 0.01f;          //How close the camera is allowed towards the player
    [SerializeField]float maxZoom = 100f;            //how far away the camera is allowed from the player
    [SerializeField] AnimationType animationType;   //the way the camera should move towards or away from the player
    private float tempCameraDistance;

    public enum AnimationType{
        linear,
        root,
        quadratic,
    };

/// <summary>
/// Find the object to rotate around.  Initialize the armLength variable.
/// </summary>
    void Start()
    {
        character = GameObject.Find("Character");
        calculateCameraArmLength();
    }

/// <summary>
/// Updates the zoom of the camera, rotates the camera around the player, and ensures it is always looking
/// at the player.
/// </summary>
    void LateUpdate(){
        snapCameraForward();
        Zoom();
        Pan();
        this.transform.LookAt(character.transform);
    }

/// <summary>
/// Rotates the camera around an imaginary sphere surrounding the player character
/// whose radius is determined by the zoom function.
/// </summary>
    void Pan(){
        if(transform.parent.transform.GetChild(0).GetComponent<MouseContext>().getMouseContext() == MouseContext.mouseContext.menu){
            return;
        }
        float xpan = Input.GetAxis("Mouse X");
        float ypan = Input.GetAxis("Mouse Y");

        //detect panning of mouse
        if(ypan != 0){
            phi = Mathf.Clamp(phi + ypan * panSpeed,    //magic value prevents jumping and fixing of
            0.0001f,                                    //camera angles/rotation to 0 and pi
            (float)Math.PI - 0.0001f);                  // 
        }
        if(xpan != 0){
            theta = theta + xpan * panSpeed;
        }

   //zpos
        RenormalizeAngles();
    }

/// <summary>
/// Prevents data overflow from players over-rotating the camera
/// </summary>
    void RenormalizeAngles(){
        if(theta > 2 * (float)Math.PI){
            theta = theta - 2 * (float)Math.PI;
        }
        if(theta < 2 * (float)Math.PI){
            theta = theta + 2 * (float)Math.PI;
        }
    }

/// <summary>
/// Zooms the camera in based on the animation type.  Linear animations zoom the
/// camera in an even amount each frame.  Root animation updates animations by
/// decreasing amounts every frame.  Quadratic animations start slowly and ramp
/// velocity at the end of the animation.
/// </summary>
    void Zoom(){
        if(transform.parent.transform.GetChild(0).GetComponent<MouseContext>().getMouseContext() == MouseContext.mouseContext.menu){
            return;
        }
        int direction = (int)Input.GetAxis("Mouse ScrollWheel");
        if(direction !=0){
            coroutine = animateZoom(-direction);
            StartCoroutine(coroutine);
        }
        setCameraPosition();
    }

/// <summary>
/// Manages the actual zooming animation coroutine.
/// </summary>
/// <param name="direction">Whether the camera zooms towards or away from the player</param>
/// <returns>updates each frame</returns>
    private IEnumerator animateZoom(int direction){
        float distanceRemaining = zoomDistance;
        int frameNum = 1;

        while(distanceRemaining > zoomStopThreshold){
            switch((int)animationType){
                case 0: //linear animation
                    if(overExtendedCamera(direction)){
                        distanceRemaining = 0;
                        StopCoroutine(coroutine);
                        break;
                    }
                    armLength = armLength + (distanceRemaining / numberOfZoomingFrames * direction);
                    distanceRemaining = distanceRemaining - (distanceRemaining / numberOfZoomingFrames);
                    break;
                case 1: //square root animation
                    if(overExtendedCamera(direction)){
                        distanceRemaining = 0;
                        StopCoroutine(coroutine);
                        break;
                    }
                    armLength = armLength + direction * rootDistanceCalculation(frameNum, distanceRemaining);
                    distanceRemaining = distanceRemaining - rootDistanceCalculation(frameNum, distanceRemaining);
                    frameNum++;
                    break;
                case 2: //quadratic animation
                    if(overExtendedCamera(direction)){
                        distanceRemaining = 0;
                        StopCoroutine(coroutine);
                        break;
                    }
                    armLength = armLength + direction * quadraticDistanceCalculation(frameNum, distanceRemaining);
                    distanceRemaining = distanceRemaining - quadraticDistanceCalculation(frameNum, distanceRemaining);
                    frameNum++;
                    break;
            }

            yield return null;
        }


    }

/// <summary>
/// Checks whether or not the camera has reached the min camera zoom or max camera zoom.
/// </summary>
/// <param name="direction">Whether the camera is zooming in or out</param>
/// <returns>Returns true if the camera has reached either bound.  Returns false otherwise</returns>
    private bool overExtendedCamera(int direction){
    if(armLength <= minZoom && direction == -1){
        armLength = minZoom;
        return true;
    }
    if(armLength >= maxZoom && direction == 1){
        armLength = maxZoom;
        return true;
    }
    return false;
    }

/// <summary>
/// A helper function that calculates the distance the camera should move on the next
/// frame for a root animation type.
/// </summary>
/// <param name="frameNum">The current frame the animation is running</param>
/// <param name="distanceRemaining">How much further the animation needs to cover</param>
/// <returns>Distance the camera should move this frame</returns>
    private float rootDistanceCalculation(int frameNum, float distanceRemaining){
        return (distanceRemaining * (float)Math.Sqrt(frameNum / numberOfZoomingFrames)
        - distanceRemaining * (float)Math.Sqrt((frameNum - 1) / numberOfZoomingFrames));
    }

/// <summary>
/// A helper function that calculates the distance the camera should move this frame
/// in a quadratic animation type.
/// </summary>
/// <param name="frameNum">The current frame the animation is running</param>
/// <param name="distanceRemaining">Distance the animation still needs to cover</param>
/// <returns>The distance the camera should  move this frame.</returns>
    private float quadraticDistanceCalculation(int frameNum, float distanceRemaining){
        return (distanceRemaining * (float)Math.Pow((frameNum / numberOfZoomingFrames),2)
        - distanceRemaining * (float)Math.Pow(((frameNum - 1) / numberOfZoomingFrames),2));
    }

/// <summary>
/// Helper function that recalculates the camera arm length
/// </summary>
    private void calculateCameraArmLength(){
        armLength = (float)Math.Sqrt(
            Math.Pow(this.transform.localPosition.x,2) +
            Math.Pow(this.transform.localPosition.y,2) +
            Math.Pow(this.transform.localPosition.z,2)
        );
    }

/// <summary>
/// Returns the angle the camera is viewing the player in the X-Z plane
/// </summary>
/// <returns>Camera angle about the y-axis, in the X-Z plane</returns>
    public float getTheta(){
        return theta;
    }

/// <summary>
/// Returns the angle the camera is viewing the player against the y axis
/// </summary>
/// <returns>float value representing the angle against the y-axis</returns>
    public float getPhi(){
        return phi;
    }

    private void snapCameraForward(){
        RaycastHit hit;
        int layerMask = (0b00011); //mask layers 8 and 16
        Debug.DrawRay(transform.parent.GetChild(0).position, snapCameraRaycastDirection(), Color.red, 1f);
        if(Physics.Raycast(transform.parent.GetChild(0).position, snapCameraRaycastDirection(), out hit, armLength, layerMask)){
            print(hit.transform.gameObject);
            tempCameraDistance = hit.distance;
            return;
        }
        tempCameraDistance = armLength;
    }

    private Vector3 snapCameraRaycastDirection(){
        Vector3 returnVector = new Vector3(
            transform.position.x - transform.parent.GetChild(0).position.x,
            transform.position.y - transform.parent.GetChild(0).position.y,
            transform.position.z - transform.parent.GetChild(0).position.z
        );
        return returnVector;
    }

    private void setCameraPosition(){
        //update the camera position
        this.transform.localPosition = new Vector3(
            (float)Math.Sin(theta) * (float)Math.Sin(phi) * tempCameraDistance,      //xpos
            (float)Math.Cos(phi) * tempCameraDistance,                               //ypos
            (float)Math.Cos(theta) * (float)Math.Sin(phi) * tempCameraDistance);  
    }
}}