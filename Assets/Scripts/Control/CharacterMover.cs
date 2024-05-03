using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/2/2024
Version:        1.0
Description:    This Code interfaces with the Character Controller component and handles
                basic character movement, like walking, strafing, and jumping.
ChangeLog:      V 0.1.0 -- 4/19/2024
                    --Imported code from unity docs under CharacterController.Move
                    --Altered code to move player in cameras forward facing direction
                    --added strafing
                    --NYI: Make Jumping more snappy by increasing and decreasing gravity with inputs
                    --Dev time: 2 Hours
                V 0.1.1 -- 4/22/2024
                    --Fixed a divide by zero error while the Vector3 move is being normalized
                    --Fixed an error where the player could rotate their character while the game
                    is paused
                    --Dev time: 0 hours.
                V 0.1.2 -- 4/23/2024
                    --Added a characterState that will update the gravity the player
                    experiences for different gameplay moments, like being pulled in by the
                    grapple hook.
                V 0.1.3 -- 4/24/2024
                    --jump is now its own method so other classes may call it
                    --Player now experiences different gravity while holding the space
                    bar after jumping
                    --Dev time: 0.25 Hours
                V 1.0 5/2/3024
                    --Added momentum to the player from velocity events.
                    --touching the floor now cancel momentum events.
                    --dev time: 1 hour
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
    private const int playerMass = 120;
    private bool hasMomentum = false;
    [SerializeField] float maximumVelocity = 1f;


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
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            setCharacterState(CharacterState.normalGamePlay);
            if(hasMomentum){
                playerVelocity = new Vector3(0, 0, 0);
                hasMomentum = false;
            }
            else playerVelocity.y = 0f;
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
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            jump();
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
    public void jump(){
        setCharacterState(CharacterState.jumping);
        isJumping = true;
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
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
        if(velocity.sqrMagnitude > maximumVelocity){
            velocity = velocity.normalized * maximumVelocity;
        }
        playerVelocity += velocity * playerMass;
        hasMomentum = true;
        print(velocity + " magnitude of " + velocity.magnitude);
    }
}

}