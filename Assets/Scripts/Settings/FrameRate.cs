using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/14/2024
Version:        0.1.0
Description:    Sets the target framerate
ChangeLog:      V 0.1.0 -- 5/14/2024
                    --NYI: make framerate changeable
                    --Dev time: 0.25 Hours
*/
namespace Settings{
public class FrameRate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}}
