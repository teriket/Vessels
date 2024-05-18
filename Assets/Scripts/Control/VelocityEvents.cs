using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/12/2024
Version:        0.1.0
Description:    Handles velocity-giving events the player might experience,
                especially velocity with a y component, like jumping or being
                launched from the grapple hook.  Also handles the amount of
                gravity the player experiences
ChangeLog:      V 0.1.0 -- 5/12/2024
                    --Code pulled apart from the character mover v1.2.0
                    --NYI: text here
                    --Dev time: 0.25 Hours
*/
namespace Control{
public class VelocityEvents : MonoBehaviour
{
    private CharacterMover characterMover;
    private CharacterController controller;                 
    private bool groundedPlayer;
    [SerializeField]float jumpHeight;            //how heigh the player should jump
    private Vector3 floorNormalDir;
    [SerializeField] float launchVelocity;
    [SerializeField][Range(0,1)] float slideAngle;
    [SerializeField][Range(0, 10)]float slideSpeedMultiplier;
    private IEnumerator coroutine;
    CharacterState characterState;
    private bool isJumping;
    bool accelerateGravityCoroutineRunning = false;

    void Start(){
        characterState = this.GetComponent<CharacterState>();
        controller = gameObject.GetComponent<CharacterController>(); 
        characterMover = this.GetComponent<CharacterMover>();
    }

    void Update(){
        //negate velocity events if the character touches the ground
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && characterState.playerVelocity.y < 0 && !slide())
        {
            characterState.setGravityState(CharacterState.CharacterGravity.normalGamePlay);
            zeroVelocity();
        }

        //increase gravity if the space key is released to increase jumping responsiveness
        if(Input.GetButtonUp("Jump") && isJumping){
            characterState.setGravityState(CharacterState.CharacterGravity.normalGamePlay);
        }

        //jumps with the correct input and reaccelerates gravity after the peak of the jump
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            jump();
        }
        if(Input.GetButtonUp("Jump") && !groundedPlayer && isJumping){
            characterState.setGravityState(CharacterState.CharacterGravity.normalGamePlay);
            accelerateGravityCoroutineRunning = false;
        }

        //pulls the player down
        characterState.playerVelocity.y += characterState.gravityValue * Time.deltaTime;
    }

/// <summary>
/// Jumps the player.  The player is lighter until they release the jumping button.
/// </summary>
    public void jump(float jumpMultiplier = 1, float xDir = 0, float yDir = 1, float zDir = 0){
        Vector3 jumpDirection = new Vector3(xDir, yDir, zDir).normalized;
        characterState.setGravityState(CharacterState.CharacterGravity.jumping);
        isJumping = true;
        characterState.playerVelocity += jumpDirection *jumpMultiplier * Mathf.Sqrt(jumpHeight * -3.0f * characterState.gravityValue);
        coroutine = accelerateGravity(Mathf.Sqrt(jumpHeight * -3.0f * characterState.gravityValue)/ -characterState.jumpingGravityValue);
        if(!accelerateGravityCoroutineRunning){
        StartCoroutine(coroutine);
        }
    }

/// <summary>
/// Zeroes the players movement
/// </summary>
    public void zeroVelocity(){
        characterState.playerVelocity = new Vector3(0, 0, 0);
    }

/// <summary>
/// Adds momentum to a velocity event, like the grapple hook, launching the player through
/// the air.
/// </summary>
/// <param name="velocity"></param>
    public void addMomentum(Vector3 velocity){
        characterState.playerVelocity += velocity.normalized * launchVelocity;
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

            characterState.playerVelocity += slideDir * Time.deltaTime * slideSpeedMultiplier;
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
    
/// <summary>
/// increases gravity when the player has reached the highest point of their jump
/// </summary>
/// <param name="waitTime">how long it takes the player to reach the heighest point of the jump</param>
/// <returns></returns>
    private IEnumerator accelerateGravity(float waitTime){
        yield return new WaitForSeconds(waitTime);
        accelerateGravityCoroutineRunning = false;
        if(characterState.characterGravity == CharacterState.CharacterGravity.jumping){
            characterState.setGravityState(CharacterState.CharacterGravity.normalGamePlay);
        }
    }

}}