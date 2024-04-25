using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           4/24/2024
Version:        0.1.0
Description:    Sticks the player to the wall the grapple hook belongs to.  The player
                is allowed to jump to exit this state.
ChangeLog:      V 0.1.0 -- 4/24/2024
                    --The grapple now childs itself to the target
                    --The characterControllers velocity is zeroed every frame through
                    the charactermover to reduce jitter.
                    --the player transform is snapped to the grapple every frame.
                    --Dev time: dev time is rolled into the grapple hook master class
*/
namespace Abilities{
public class PlayerSnappedState : IState
{
    GrappleHook owner;
    GameObject collidedObject;

    public PlayerSnappedState(GrappleHook owner){
        this.owner = owner;
    }

    public void enter(){}
/// <summary>
/// cancels any velocity the player has through the charactermover to reduce jitter.
/// Sets the players position to the grapples position every frame.  Pressing jump
/// or releasing the RMB switches the player back to the reeling state.
/// </summary>
    public void execute(){
        owner.player.transform.GetChild(0).GetComponent<CharacterMover>().zeroVelocity();
        owner.player.transform.GetChild(0).position = owner.transform.position;
        if(Input.GetButtonDown("Jump")){
            owner.player.transform.GetChild(0).GetComponent<CharacterMover>().jump();
            owner.currentState.changeState(new ReelingGrappleState(owner));
        }
        if(Input.GetMouseButtonUp(1)){
            owner.currentState.changeState(new ReelingGrappleState(owner));
        }
    }
    public void exit(){

    }
}
}