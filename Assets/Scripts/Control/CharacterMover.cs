using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/8/2024
Version:        1.1.0
Description:    This Code interfaces with the Character Controller component and handles
                basic character movement, like walking, strafing, and jumping.  Now also
                creates velocity events on the player, like being catapulted from the sling
                shot or changing the direction the player jumps if they are clinging to a wall.
ChangeLog:      
                V 1.1.0 5/8/2024
                    --expanded functionality of jump function so it now can specify a direction
                    vector.
                    --Player input will now reduce velocity on the x and z axis
                    --NYI: merge jump and addmomentum?
                V 1.2.0 5/10/2024
                    --gravity now scales up after the player reaches the max jump height
                    --NYI: player overshoots and clips into the terrain while following
                    --NYI: isJumping is redundant to character state
                    --NYI: This class has taken on too much responsibility.  Tease out vertical movement
                    and momentum from this class.
*/

namespace Control{
public class CharacterMover : MonoBehaviour
{
    private CharacterController controller;             //the character controller component on the character
    private Vector3 playerVelocity;                     //how fast the character is moving
    private bool groundedPlayer;                        //whether or not the player is touching the ground
    private float gravityValue;
    [SerializeField]float playerSpeed = 2.0f;           //how fast the player should move
    [SerializeField]float jumpHeight = 1.0f;            //how heigh the player should jump
    [SerializeField]float defaultGravityValue = -16f;   //the strength of gravity during normal gameplay
    [SerializeField]float grapplingGravityValue = 0f;   //the strength of gravity while reeling on the grapple hook
    [SerializeField]float jumpingGravityValue = -9.81f; //the momentary lightness a player feels until they release the spacebar
    private CameraController cameraController;          //reference to the camera controller; determines the forward direction.
    private bool isJumping;
    public enum CharacterState{
        normalGamePlay,
        grappling,
        jumping
    }
    public CharacterState characterState;
    private Vector3 floorNormalDir;
    [SerializeField] float maximumLaunchVelocity = 1f;
    [SerializeField][Range(0,1)] float slideAngle;
    [SerializeField][Range(0, 10)]float slideSpeedMultiplier;
    [SerializeField][Range(0, 10)]float joltDownFromJump;
    [SerializeField]float overrideVelocityMultiplier;
    bool accelerateGravityCoroutineRunning = false;
    private IEnumerator coroutine;


/// <summary>
/// Cache the camera controller and the character controller
/// </summary>
    private void Start()
    {
        setCharacterState(CharacterState.normalGamePlay);
        controller = gameObject.GetComponent<CharacterController>();
        cameraController = this.transform.parent.GetComponentInChildren<CameraController>();
    }

/// <summary>
/// Checks the player for collisions with the ground.  Moves the player if they press
/// the movement buttons.  The player jumps if they press the jump key.
/// </summary>
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue, 2f);
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0 && !slide())
        {
            setCharacterState(CharacterState.normalGamePlay);
            zeroVelocity();
        }

        if(Input.GetButtonUp("Jump") && isJumping){
            setCharacterState(CharacterState.normalGamePlay);
        }
        Vector3 move = new Vector3();

        if(GetComponent<MouseContext>().getMouseContext() != MouseContext.mouseContext.menu){
            move = forwardMovement() + strafe();
        }

        if (move != Vector3.zero)
        {
            move = move / magnitude(move);
            controller.Move(move * Time.deltaTime * playerSpeed);
            gameObject.transform.forward = move;
            reducePlayerVelocity();
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            jump();
        }
        if(Input.GetButtonUp("Jump") && !groundedPlayer && characterState == CharacterState.jumping){
            setCharacterState(CharacterState.normalGamePlay);
            accelerateGravityCoroutineRunning = false;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

/// <summary>
/// Returns a forward or backward vector if the player presses the forward or backward
/// movement keys.
/// </summary>
/// <returns>Direction vector the player should be moved.</returns>
    private Vector3 forwardMovement(){
        Vector3 move = new Vector3();
        move.x = (float)Math.Sin(cameraController.getTheta()) * -1;
        move.y = 0;
        move.z = (float)Math.Cos(cameraController.getTheta()) * -1;
        move = move * forwardBackwardMovementInput();
        return move;
    }

/// <summary>
/// A helper function that determines whether the player is pressing the forward or
/// backward key.
/// </summary>
/// <returns>1 if the player presses forward, -1 if the player presses backward</returns>
    private int forwardBackwardMovementInput(){
        if(Input.GetKey("w")){
            return 1;
        }
        if(Input.GetKey("s")){
            return -1;
        }
        return 0;
    }

/// <summary>
/// Returns a vector perpindicular to the forward plane of the character for strafing
/// left and right.
/// </summary>
/// <returns>a perpindicular vector to the forward direction.</returns>
    private Vector3 strafe(){
        Vector3 move = new Vector3();
        if(Input.GetKey("d")){
            move.x = (float)Math.Cos(cameraController.getTheta()) * -1;
            move.y = 0;
            move.z = (float)Math.Sin(cameraController.getTheta());
            return move;
        } 
        if(Input.GetKey("a")){
            move.x = (float)Math.Cos(cameraController.getTheta());
            move.y = 0;
            move.z = (float)Math.Sin(cameraController.getTheta()) * -1;
            return move;
        } 
        return move;
    }

/// <summary>
/// A helper function that determines the magnitude of a vector.  Useful in normalizing
/// player movement direction if they press multiple inputs at the same time.
/// </summary>
/// <param name="vector3">The vector to be normalized</param>
/// <returns>the magnitude of vector3</returns>
    private float magnitude(Vector3 vector3){
        return(
            (float)Math.Sqrt(
                Math.Pow(vector3.x, 2) +
                Math.Pow(vector3.y, 2) +
                Math.Pow(vector3.z, 2)
            )
        );
    }

/// <summary>
/// Handles the way the player should fall during different gameplay moments, like while
/// holding jump, grapple hooking, and normal gameplay.
/// </summary>
    private void characterStateUpdate(){
        switch(characterState){
            case CharacterState.normalGamePlay:
                gravityValue = defaultGravityValue;
                break;
            case CharacterState.grappling:
                gravityValue = grapplingGravityValue;
                break;
            case CharacterState.jumping:
                gravityValue = jumpingGravityValue;
                break;
            default:
                gravityValue = defaultGravityValue;
                break;
        }
    }
    
/// <summary>
/// allows other scripts to set the current player state for gravity purposes.
/// </summary>
/// <param name="characterState">declared CharacterMover.CharacterState.state,
/// states include normalGamePlay, reeling</param>
    public void setCharacterState(CharacterState characterState){
        this.characterState = characterState;
        characterStateUpdate();
    }

/// <summary>
/// Jumps the player.  The player is lighter until they release the jumping button.
/// </summary>
    public void jump(float jumpMultiplier = 1, float xDir = 0, float yDir = 1, float zDir = 0){
        Vector3 jumpDirection = new Vector3(xDir, yDir, zDir).normalized;
        setCharacterState(CharacterState.jumping);
        isJumping = true;
        playerVelocity += jumpDirection *jumpMultiplier * Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        coroutine = accelerateGravity(Mathf.Sqrt(jumpHeight * -3.0f * gravityValue)/ -jumpingGravityValue);
        if(!accelerateGravityCoroutineRunning){
        StartCoroutine(coroutine);
        }
    }

/// <summary>
/// Zeroes the players movement
/// </summary>
    public void zeroVelocity(){
        playerVelocity = new Vector3(0, 0, 0);
    }

/// <summary>
/// Adds momentum to a velocity event, like the grapple hook, launching the player through
/// the air.  Momentum added is limited so the player is not launched into space.
/// </summary>
/// <param name="velocity"></param>
    public void addMomentum(Vector3 velocity){
        playerVelocity += velocity.normalized * maximumLaunchVelocity;
    }

/// <summary>
/// Moves the player along the gradient of a slope that is too steep.
/// </summary>
/// <returns>True if the player is sliding to lock jumping and zeroing velocities</returns>
    bool slide(){
        if(floorNormalDir.y < slideAngle){
        Vector3 slideDir = new Vector3(
            floorNormalDir.x,
            -(Mathf.Pow(floorNormalDir.x, 2) + Mathf.Pow(floorNormalDir.z, 2)/floorNormalDir.y),
            floorNormalDir.z);

            playerVelocity += slideDir * Time.deltaTime * slideSpeedMultiplier;
            return true;
        }
        return false;
    }

/// <summary>
/// gets the normal of the geometry that is below the player
/// </summary>
/// <param name="hit"></param>
    void OnControllerColliderHit(ControllerColliderHit hit){
        if(hit.normal.y < slideAngle && hit.point.y < this.transform.position.y){
            floorNormalDir = hit.normal;
            }
        else floorNormalDir = new Vector3(0, 1, 0);
    }

    void reducePlayerVelocity(){
        playerVelocity = new Vector3(playerVelocity.x * overrideVelocityMultiplier, playerVelocity.y, playerVelocity.z * overrideVelocityMultiplier);
        if(playerVelocity.magnitude < 0.1f){
            zeroVelocity();
        }
    }

    private IEnumerator accelerateGravity(float waitTime){
        yield return new WaitForSeconds(waitTime);
        accelerateGravityCoroutineRunning = false;
        if(characterState == CharacterState.jumping){
            setCharacterState(CharacterState.normalGamePlay);
            print(gravityValue);
        }
    }


}}