using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           4/23/2024
Version:        0.1.0
Description:    Sends a message about collision events to Grapple Hook states
ChangeLog:      V 0.1.0 -- 4/23/2024
                    --Implemented Code
                    --Dev time: 0 Hours
*/
public class CollisionMessenger : MonoBehaviour
{
    public delegate void Collided(Collision collision);
    public Collided collisionEvent;

    void OnCollisionEnter(Collision collision){
        collisionEvent(collision);
    }
}
