using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/8/2024
Version:        0.2.0
Description:    Sends a message about collision events to Grapple Hook states.  Also keeps
                track of the normal of any object the player is colliding with.
ChangeLog:      
                V 0.2.0 -- 5/8/2024
                    --Collision messenger now tracks the normal of the object collided with
                    continuously
*/
public class CollisionMessenger : MonoBehaviour
{
    public delegate void Collided(Collision collision);
    public Collided collisionEvent;
    public Vector3 collisionNormal {get; private set;}

    void OnCollisionEnter(Collision collision){
        collisionEvent(collision);
    }

    public void OnCollisionStay(Collision collision){
        collisionNormal = collision.contacts[0].normal;
    }
}
