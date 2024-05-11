using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
Author:         Tanner Hunt
Date:           4/19/2024
Version:        0.1
Description:    Moves the empty that houses player-related objects directly ontop of the
                player for camera-related movements.
ChangeLog:      
*/

namespace Control{
public class FollowEmpty : MonoBehaviour
{
    Transform gameObjectToFollow;

/// <summary>
/// Cache a reference to the gameobject that should be followed
/// </summary>
    void Start()
    {
        gameObjectToFollow = this.transform.Find("Character");
    }

/// <summary>
/// Updates the position to be directly on top of the gameobject to be followed
/// </summary>
    void Update()
    {
        this.transform.position = gameObjectToFollow.position;
    }
}
}