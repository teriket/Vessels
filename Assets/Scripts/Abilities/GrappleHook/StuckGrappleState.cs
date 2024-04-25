using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           4/24/2024
Version:        0.1.0
Description:    Behavior of the grapple hook when it has collided with a wall.  The
                grapple should stop movement, child itself to the object and keep its
                relative positioning, then drag the player towards it.
ChangeLog:      V 0.1.0 -- 4/24/2024
                    --State now disables gravity on the player
                    --State now uses the CharacterController class to move the player
                    rather than transform.position to prevent jitter
                    --NYI: tether should break if player manages to get too far away
                    --Dev time: devtime rolled into grapple hook master class.
*/
namespace Abilities{
public class StuckGrappleState : IState
{
    GrappleHook owner;
    float playerPullSpeed;
    GameObject collidedObject;
    float distance;

    public StuckGrappleState(GrappleHook owner, Collision collision){
        this.owner = owner;
        collidedObject = collision.gameObject;
    }

/// <summary>
/// childs the grapple hook to the gameobject it connects with.  Clears all previous
/// accrued velocity.
/// </summary>
    public void enter(){
        owner.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        owner.player.transform.GetChild(0).GetComponent<CharacterMover>().
            setCharacterState(CharacterMover.CharacterState.grappling);
        owner.transform.SetParent(collidedObject.transform);
    }
    
/// <summary>
/// pulls the player towards the stuck grapple hook.  releasing the mouse button changes
/// the state to the reeling state.  Getting close enough to the grapple will snap
/// the player onto the grapple and change the state to the snapped state.
/// </summary>
    public void execute(){
        distance = Vector3.Distance(owner.transform.position, owner.player.transform.position);
        playerPullSpeed = owner.playerPullSpeed * Time.deltaTime;
        owner.player.transform.GetChild(0).GetComponent<CharacterController>().
            Move(moveToVector());

        if(Input.GetMouseButtonUp(1)){
            owner.currentState.changeState(new ReelingGrappleState(owner));
        }
        if(Vector3.Distance(owner.transform.position, owner.player.transform.position) < 0.1){
            owner.currentState.changeState(new PlayerSnappedState(owner));
        }
    }
    public void exit(){}

/// <summary>
/// A helper function that calculates the direction the player should be pulled towards
/// the stuck grapple hook.
/// </summary>
/// <returns>A vector3 direction the player is pulled</returns>
    private Vector3 moveToVector(){
        Vector3 move = owner.transform.position - owner.player.transform.position;
        move = move * Time.deltaTime * owner.playerPullSpeed / distance;
        return move;
    }
}
}