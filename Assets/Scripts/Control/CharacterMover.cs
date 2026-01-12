using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/12/2024
Version:        2.0.0
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
                V 2.0.0 -- 5/12/2024
                    --Refactored code and removed responsibility of jumping and other velocity 
                    events from this class
                    --removed magnitude helper function in favor of built-in funcitonality
                    --removed player velocity from this class, it is now referenced from the
                    characterstate class
                    --move stays in this class.  the velocityEvents class also changes the
                    velocity that this method call references, but shouldn't be in both classes.
                    It seems more appropriate for it to be called here.
                    --Player now accelerates while they run, up to a maximum speed
*/

namespace Control{
public class CharacterMover : MonoBehaviour
{
    private CharacterState characterState;
    private CharacterController controller;
    private CharacterState owner;                  
    [SerializeField]float defaultPlayerSpeed = 2.0f;
    [SerializeField]float maxPlayerSpeed;
    [SerializeField]float playerAcceleration;
    private float playerSpeed;           
    private CameraController cameraController;          
    private Vector3 floorNormalDir;
    [SerializeField][Range(0,1)] float slideAngle;
    [SerializeField][Range(0, 10)]float slideSpeedMultiplier;
    [SerializeField]float overrideVelocityMultiplier;
    private IEnumerator coroutine;


/// <summary>
/// Cache the camera controller and the character controller
/// </summary>
    public void Start()
    {
        characterState = this.GetComponent<CharacterState>();
        controller = gameObject.GetComponent<CharacterController>();
        cameraController = this.transform.parent.GetComponentInChildren<CameraController>();
    }

/// <summary>
/// Checks the player for collisions with the ground.  Moves the player if they press
/// the movement buttons.  The player jumps if they press the jump key.
/// </summary>
    public void Update()
    {
        Vector3 move = new Vector3();

        if(GetComponent<MouseContext>().getMouseContext() != MouseContext.mouseContext.menu){
            move = forwardMovement() + strafe();
        }

        if (move != Vector3.zero)
        {
            playerSpeed += playerAcceleration * Time.deltaTime;
            playerSpeed = Mathf.Clamp(playerSpeed, defaultPlayerSpeed, maxPlayerSpeed);
            move = move.normalized;
            controller.Move(move * Time.deltaTime * playerSpeed);
            gameObject.transform.forward = move;
            //reducePlayerVelocity();
        }
        else playerSpeed = defaultPlayerSpeed;
        controller.Move(characterState.playerVelocity * Time.deltaTime);
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
/// Zeroes the players movement
/// </summary>
    public void zeroVelocity(){
        characterState.playerVelocity = new Vector3(0, 0, 0);
    }

/// <summary>
/// Slows down the players velocity if they hold down a movement key in the air.
/// </summary>
    private void reducePlayerVelocity(){
        characterState.playerVelocity = new Vector3(characterState.playerVelocity.x * overrideVelocityMultiplier, characterState.playerVelocity.y, characterState.playerVelocity.z * overrideVelocityMultiplier);
        if(characterState.playerVelocity.magnitude < 0.1f){
            zeroVelocity();
        }
    }

}}