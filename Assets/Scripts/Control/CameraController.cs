using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
Author:         Tanner Hunt
Date:           5/7/2024
Version:        1.0.2
Description:    This Code handles camera movements and rotations around the player character.
                panning the mouse in the x or y directions should rotate the camera and
                the scroll wheel should zoom in or out.  The camera stops moving when
                ui elements are active.
ChangeLog:      
                V 1.0.1 -- 5/3/2024
                    --Put Time.deltaTime into camera panning again to create consistency
                    between test environment and full builds.
                    -- Camera now lerps to the new camera position for smoother camera
                    movements.
                    --NYI: fix bug where camera snaps around when player strafes or backpedals
                    --Dev time: 1 hour
                V 1.0.2 -- 5/7/2024
                    --Camera now follows behind the players shoulder
                    --dev time is rolled into the camerafollower script

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
    [SerializeField]float shoulderSlide;

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
        character = transform.parent.GetChild(3).gameObject;
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
            phi = Mathf.Clamp(phi + ypan * panSpeed * Time.deltaTime,    //magic value prevents jumping and fixing of
            0.0001f,                                    //camera angles/rotation to 0 and pi
            (float)Math.PI - 0.0001f);                  // 
        }
        if(xpan != 0){
            theta = theta + xpan * panSpeed * Time.deltaTime;
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
        if(Physics.Raycast(transform.parent.GetChild(0).position, snapCameraRaycastDirection(), out hit, armLength, layerMask)){
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
    Vector3 currentPosition = this.transform.localPosition;
    Vector3 moveToPosition = new Vector3(
            (float)Math.Sin(theta) * (float)Math.Sin(phi) * tempCameraDistance,      //xpos
            (float)Math.Cos(phi) * tempCameraDistance,                               //ypos
            (float)Math.Cos(theta) * (float)Math.Sin(phi) * tempCameraDistance);
    this.transform.localPosition = Vector3.Lerp(currentPosition, moveToPosition, .5f);
    }
}}