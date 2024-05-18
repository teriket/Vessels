using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           5/15/2024
Version:        0.2.0
Description:    If the grapple hook is released or gets too far away, it should return
                to the player and transition to the idle state.
ChangeLog:      
            V 0.2.0 -- 5/15/2024
                --Now created an optional delay for the grapple hook to start reeling
                towards the player.  This prevents infinite wall climbing
*/
namespace Abilities{
public class ReelingGrappleState : IState
{
    GrappleHook owner;
    float reelSpeed;
    float cooldownStartTime;
    float delay;

    public ReelingGrappleState(GrappleHook owner, float delay = 0){
        this.owner = owner;
        this.delay = delay;
    }

/// <summary>
/// Disable the sphere collider, cancel previous momentum the grapple has accumulated,
/// change the character state back to normal gameplay (which resumes gravity),
/// and start the cooldown counter.
/// </summary>
    public void enter(){
        owner.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        owner.GetComponent<SphereCollider>().enabled = false;
        owner.player.transform.GetChild(0).GetComponent<CharacterState>().
            setGravityState(CharacterState.CharacterGravity.normalGamePlay);
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
        if(cooldownStartTime - delay > 0){
            reelSpeed = owner.reelSpeed * Time.deltaTime;    
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, owner.player.transform.position, reelSpeed * Time.deltaTime);
            if(Vector3.Distance(owner.transform.position, owner.player.transform.position) < 0.01f
            && Time.time - cooldownStartTime - delay > owner.cooldownTime){
                owner.currentState.changeState(new IdleGrappleState(owner));
        }}
    }
    public void exit(){}
}
}