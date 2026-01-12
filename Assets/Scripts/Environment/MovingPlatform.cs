using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           7/19/2024
Version:        0.1.0
Description:    A moving platform script with sinusoidal movement
ChangeLog:      V 0.1.0 -- 7/19/2024
                    --Implemented Code
                    --NYI: Child the player to an object they are standing on
                    to reduce jittery movements
*/

namespace Environment{
public class MovingPlatform : MonoBehaviour
{
    [SerializeField]float yAmplitude;
    [SerializeField]float xAmplitude;
    [SerializeField]float zAmplitude;
    [SerializeField]float speed;
    Rigidbody rb;
    void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

    void FixedUpdate()
    {
        rb.MovePosition(new Vector3(
            transform.position.x + xAmplitude * Mathf.Sin(Time.time) * Time.deltaTime * speed,
            transform.position.y + yAmplitude * Mathf.Sin(Time.time) * Time.deltaTime * speed,
            transform.position.z + zAmplitude * Mathf.Sin(Time.time) * Time.deltaTime * speed
        ));
    }

    
}
}