using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           5/8/2024
Version:        0.2.0
Description:    Sticks the player to the wall the grapple hook belongs to.  The player
                is allowed to jump to exit this state.
ChangeLog:      
                V 0.2.0 -- 5/8/2024
                    --player now jumps away from the surface they are attached to by 
                    getting the normal from the collisionMessenger
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

    public PlayerSnappedState(GrappleHook owner, Transform offset){
        this.owner = owner;
        this.offset = offset.localPosition.y;
    }

    public void enter(){
        collisionMessage = owner.transform.GetComponent<CollisionMessenger>();
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
        if(Input.GetButtonDown("Jump")){
            xJumpDir = collisionMessage.collisionNormal.x;
            zJumpDir = collisionMessage.collisionNormal.z;
            owner.player.transform.GetChild(0).GetComponent<CharacterMover>().jump(owner.bonusJumpMultiplier, xJumpDir, 2, zJumpDir);
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