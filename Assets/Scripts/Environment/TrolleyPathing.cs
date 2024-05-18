using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           4/24/2024
Version:        0.1.0
Description:    Moves the trolley from one point to the next and makes it look
                directly at the next point on the track.  Trolley points should
                not be childed directly to the trolley.
ChangeLog:      
*/
namespace Environment{
public class TrolleyPathing : MonoBehaviour
{
    [SerializeField]Transform[] positions;
    [SerializeField]float speed;
    int indexToMoveTo = 0;

/// <summary>
/// snaps the trolley to the first point on the track.
/// </summary>
    void Start(){
        transform.position = positions[0].position;
    }

    void Update()
    {
    moveToNext();
    }

/// <summary>
/// Moves the trolley to the next point on the track.  If there are no more points on the
/// array, the trolley will snap back to the start.
/// </summary>
    private void moveToNext(){
        if(Vector3.Distance(transform.position, positions[indexToMoveTo].position) < 0.1){
            if(indexToMoveTo == positions.Length - 1){
                transform.position = positions[0].position;
                indexToMoveTo = 1;
                return;
            }
            indexToMoveTo ++;
        }
        transform.position = Vector3.MoveTowards(transform.position, positions[indexToMoveTo].position, speed);
        transform.LookAt(positions[indexToMoveTo]);
    }
}}
