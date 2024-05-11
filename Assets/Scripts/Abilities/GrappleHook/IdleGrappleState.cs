using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           4/21/2024
Version:        0.1.0
Description:    How the grapple hook behaves while it isn't being utilized.  The Sphere
                Collider should be disabled, and the hook should listen for an activation
                event, then transfer to the shooting state.
ChangeLog:      
*/
namespace Abilities{
public class IdleGrappleState :  IState
{
    GrappleHook owner;

    public IdleGrappleState(GrappleHook owner){
        this.owner = owner;
    }

    public void enter(){
        owner.GetComponent<SphereCollider>().enabled = false;
    }

/// <summary>
/// keeps the grapple hook attached to the player.  Updates to the shooting state
/// if the RMB is pressed.
/// </summary>
    public void execute(){
        owner.transform.position = owner.player.transform.GetChild(0).transform.position;
        if(Input.GetMouseButtonDown(1)){
            owner.currentState.changeState(new ShootingGrappleState(owner));
        }
    }

    public void exit(){

    }
}
}