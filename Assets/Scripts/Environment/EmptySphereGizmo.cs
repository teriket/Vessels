using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/15/2024
Version:        0.1.0
Description:    Makes a sphere around an empty for visual clarity when it is selected
ChangeLog:      V 0.1.0 -- 5/15/2024
*/
public class EmptySphereGizmo : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}
