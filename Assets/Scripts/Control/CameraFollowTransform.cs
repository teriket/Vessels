using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           4/20/2024
Version:        0.1.0
Description:    The object above the characters shoulder the camera should point towards
                rotates around the player slightly ahead of the camera.  This creates
                an over the shoulder view from behind.
ChangeLog:      V 0.1.0 -- 4/20/2024
                    --Dev time: 0.5 Hours
*/
namespace Control{
public class CameraFollowTransform : MonoBehaviour
{
    [SerializeField]float distance;
    [SerializeField]float height;
    CameraController cc;
    // Start is called before the first frame update
    void Start()
    {
        cc = transform.parent.GetChild(1).GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(
            -Mathf.Sin(cc.getTheta() + (Mathf.PI / 2)) * distance,
            height,
            -Mathf.Cos(cc.getTheta() + (Mathf.PI / 2)) * distance
        );
    }
}}
