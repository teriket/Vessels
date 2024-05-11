using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           4/24/2024
Version:        0.1.1
Description:    Shoots a grapple hook in the direction the player is facing up to a
                distance.  If the hook collides with an object it will stick to it, and
                pull the player in towards the grappling hook.  Releasing the RMB will
                free the player from this effect and reel the grapple hook back into
                place.  Allowing the player close enough to the stuck grappling hook
                will allow them to hold onto the wall.
ChangeLog:      
*/
namespace Abilities{
public class GrappleHook : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private bool isEnabled = false;
    public GameObject player {get; private set;}
    public StateMachine currentState = new StateMachine();
    [SerializeField]public float hookVelocity = 10f;
    [SerializeField]public float terminalHookVelocity = 100f;
    [SerializeField]public float maxHookDistane = 25f;
    [SerializeField]public float reelSpeed = 10f;
    [SerializeField]public float playerPullSpeed = 10f;
    [SerializeField]public float cooldownTime = 2f;
    [SerializeField]public float bonusJumpMultiplier;
    [SerializeField]public float playerSnapDistance;

    void Start(){
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = isEnabled;
        player = GameObject.Find("Your Character").gameObject;
        currentState.changeState(new IdleGrappleState(this));
    }

    void Update()
    {
        currentState.update();

    }


}
}