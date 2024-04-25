using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           4/24/2024
Version:        0.1.1
Description:    If the grapple hook is released or gets too far away, it should return
                to the player and transition to the idle state.
ChangeLog:      V 0.1.0 -- 4/22/2024
                    --Entering this state now cancels all previous velocity
                    --The Grapple hook disables its sphere collider
                    --The grapple hook moves towards the player
                    --The state returns to idle when it's close enough to the player
                    --Dev time: dev time is rolled into grapple hook master class
                V 0.1.1 -- 4/24/2022
                    --Reeling the grapple hook now incurs a cooldown to prevent
                    crashing of the game and clipping walls.
                    --NYI: state should unchild the grapple from the collision object
                    if it came from the stuck state.
*/
namespace Abilities{
public class ReelingGrappleState : IState
{
    GrappleHook owner;
    float reelSpeed;
    float cooldownStartTime;

    public ReelingGrappleState(GrappleHook owner){
        this.owner = owner;
    }

/// <summary>
/// Disable the sphere collider, cancel previous momentum the grapple has accumulated,
/// change the character state back to normal gameplay (which resumes gravity),
/// and start the cooldown counter.
/// </summary>
    public void enter(){
        owner.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        owner.GetComponent<SphereCollider>().enabled = false;
        owner.player.transform.GetChild(0).GetComponent<CharacterMover>().
            setCharacterState(CharacterMover.CharacterState.normalGamePlay);
        cooldownStartTime = Time.time;
        if(owner.transform.parent != null){
            owner.transform.SetParent(null);
        }
    }

/// <summary>
/// move the grapple back towards the player.  Return to idle state when the grapple hook
/// has both returned to the player and the appropriate cooldown period has passed.
/// </summary>
    public void execute(){
        reelSpeed = owner.reelSpeed * Time.deltaTime;    
        owner.transform.position = Vector3.MoveTowards(owner.transform.position, owner.player.transform.position, reelSpeed);
        if(Vector3.Distance(owner.transform.position, owner.player.transform.position) < 0.01f
        && Time.time - cooldownStartTime > owner.cooldownTime){
            owner.currentState.changeState(new IdleGrappleState(owner));
        }
    }
    public void exit(){}
}
}