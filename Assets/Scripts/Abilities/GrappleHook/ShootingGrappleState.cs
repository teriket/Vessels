using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           4/22/2024
Version:        0.1.0
Description:    How the grapple hook should move in air.  Upon collision, it should 
                disable the sphere collider and child itself to the collided object,
                then transfer to the stuck state.  If the player lets go of the 
                right mouse button or it gets too far away, it should transfer to the 
                reeling state.
ChangeLog:      V 0.1.0 -- 4/22/2024
                    --Grapple hook now shoots in the forward direction of the camera
                    --Grapple hook now accelerates as the player holds the RMB down
                    until it reaches terminal velocity.
                    --The state now changes if the Grapple hook collides with an object 
                    to the StuckGrappleState
                    --The Grapple Hook now switches to the ReelingGrappleState if it overextends
                    past the max extension distance or the player releases the RMB
                    --NYI: Animation styles for how the grapple hook accelerates
                    --Dev time: dev time is wrapped in the Grapple Hook master class
*/
namespace Abilities{
public class ShootingGrappleState :  IState
{
    GrappleHook owner;
    Vector3 forceDirection;
    Rigidbody rb;

    public ShootingGrappleState(GrappleHook owner){
        this.owner = owner;
    }

/// <summary>
/// Cache variables.  Subscribe to collision events in the collision messenger.
/// </summary>
    public void enter(){
        owner.GetComponent<SphereCollider>().enabled = true;
        rb = owner.GetComponent<Rigidbody>();
        forceDirection = calculateForceDirection();
        owner.GetComponent<CollisionMessenger>().collisionEvent += collision;
    }

/// <summary>
/// Continuously adds force in a direction using the rigidbody while the RMB is held down until the grapple 
/// hook reaches terminal velocity.  If the grapple hook doesn't hit anything or the
/// RMB is released the grapple hook returns to the player.
/// </summary>
    public void execute(){
        if(Input.GetMouseButton(1) && rb.velocity.magnitude <= owner.terminalHookVelocity){
            rb.AddForce(forceDirection * owner.hookVelocity);
        }
        if(Vector3.Distance(owner.transform.position, owner.player.transform.GetChild(0).transform.position) > owner.maxHookDistane){
            owner.currentState.changeState(new ReelingGrappleState(owner));
        }
        if(Input.GetMouseButtonUp(1)){
            owner.currentState.changeState(new ReelingGrappleState(owner));
        }
    }

    public void exit(){}

/// <summary>
/// Switches the state to the stuck grapple state if the grapple hook collides with
/// anything other than the player.
/// </summary>
/// <param name="collision"></param>
    void collision(Collision collision){
        if (collision.gameObject != owner.gameObject){
            owner.currentState.changeState(new StuckGrappleState(owner, collision));
        }
    }

/// <summary>
/// A helper function that decides the direction the grapple hook should shoot out.
/// </summary>
/// <returns>A Vector3 direction opposite the camera position.</returns>
    private Vector3 calculateForceDirection(){
        CameraController cameraController = owner.player.transform.GetChild(1).GetComponent<CameraController>();
        return cameraController.transform.forward;
    }
}
}