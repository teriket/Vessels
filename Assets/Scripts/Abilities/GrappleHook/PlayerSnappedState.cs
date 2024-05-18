using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           5/12/2024
Version:        0.3.0
Description:    Sticks the player to the wall the grapple hook belongs to.  The player
                is allowed to jump to exit this state.
ChangeLog:      
                V 0.2.0 -- 5/8/2024
                    --player now jumps away from the surface they are attached to by 
                    getting the normal from the collisionMessenger
                V 0.3.0 -- 5/12/2024
                    --early upgrade testing of a boosted jump off of walls
*/
namespace Abilities{
public class PlayerSnappedState : IState
{
    GrappleHook owner;
    GameObject collidedObject;
    float offset;
    float xJumpDir;
    float zJumpDir;
    CollisionMessenger collisionMessage;
    float jumpBoost;
    bool wallJumpBoostUnlocked = false;
    float reelingDelay;

    public PlayerSnappedState(GrappleHook owner, Transform offset){
        this.owner = owner;
        this.offset = offset.localPosition.y;
    }

    public void enter(){
        collisionMessage = owner.transform.GetComponent<CollisionMessenger>();
        jumpBoost = owner.bonusJumpMultiplier;
        reelingDelay = owner.reelingGrappleDelay;
    }
/// <summary>
/// cancels any velocity the player has through the charactermover to reduce jitter.
/// Sets the players position to the grapples position every frame.  Pressing jump
/// or releasing the RMB switches the player back to the reeling state.
/// </summary>
    public void execute(){
        
        owner.player.transform.GetChild(0).GetComponent<CharacterMover>().zeroVelocity();
        owner.player.transform.GetChild(0).position = new Vector3(
            owner.transform.position.x,
            owner.transform.position.y - offset,
            owner.transform.position.z
        );
        if(Input.GetButtonDown("Jump") && !wallJumpBoostUnlocked){
            boostJump();
        }
        if(Input.GetButton("Jump")){
            jumpBoost += owner.jumpBoostAccrualRate * Time.deltaTime;
            jumpBoost = Mathf.Clamp(jumpBoost, 0, owner.maxJumpBoost);
        }
        if(Input.GetButtonUp("Jump")){
            boostJump();
        }
        if(Input.GetMouseButtonUp(1)){
            owner.currentState.changeState(new ReelingGrappleState(owner, reelingDelay));
        }
    }

    private void boostJump(){
            xJumpDir = collisionMessage.collisionNormal.x;
            zJumpDir = collisionMessage.collisionNormal.z;
            owner.player.transform.GetChild(0).GetComponent<VelocityEvents>().jump(jumpBoost, xJumpDir, 2, zJumpDir);
            owner.currentState.changeState(new ReelingGrappleState(owner, reelingDelay));
    }
    public void exit(){

    }

}
}