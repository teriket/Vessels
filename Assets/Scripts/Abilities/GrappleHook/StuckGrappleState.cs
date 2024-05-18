using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           5/2/2024
Version:        0.1.2
Description:    Behavior of the grapple hook when it has collided with a wall.  The
                grapple should stop movement, child itself to the object and keep its
                relative positioning, then drag the player towards it.
ChangeLog:      
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
        owner.player.transform.GetChild(0).GetComponent<CharacterState>().
            setGravityState(CharacterState.CharacterGravity.grappling);
        owner.transform.SetParent(collidedObject.transform);
//        owner.transform.localScale = new Vector3(1/owner.transform.parent.lossyScale.x, 1/owner.transform.parent.lossyScale.y, 1/owner.transform.parent.lossyScale.z);
    }
    
/// <summary>
/// pulls the player towards the stuck grapple hook.  releasing the mouse button changes
/// the state to the reeling state.  Getting close enough to the grapple will snap
/// the player onto the grapple and change the state to the snapped state.
/// </summary>
    public void execute(){
        distance = Vector3.Distance(owner.transform.position, owner.player.transform.position);
        playerPullSpeed = owner.playerPullSpeed * Time.deltaTime;
        owner.player.transform.GetChild(0).GetComponent<CharacterController>().Move(moveToVector());
        if(Input.GetMouseButtonUp(1)){
            owner.currentState.changeState(new ReelingGrappleState(owner));
            owner.player.transform.GetChild(0).GetComponent<VelocityEvents>().addMomentum(moveToVector());
        }
        if(Vector3.Distance(owner.transform.position, owner.player.transform.position) < owner.playerSnapDistance){
            owner.currentState.changeState(new PlayerSnappedState(owner, owner.player.transform.GetChild(0)));
        }
        if(Vector3.Distance(owner.transform.position, owner.player.transform.GetChild(2).position) < owner.playerSnapDistance )
        {
            owner.currentState.changeState(new PlayerSnappedState(owner, owner.player.transform.GetChild(2)));

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
        move = move * playerPullSpeed / distance;
        return move;
    }
}
}