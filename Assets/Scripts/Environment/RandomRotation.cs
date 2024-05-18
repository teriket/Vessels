using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/12/2024
Version:        0.1.0
Description:    Slowly rotates an object and changes rotation direction periodically
ChangeLog:      V 0.1.0 -- 5/12/2024

*/
namespace Environment{
public class RandomRotation : MonoBehaviour
{
    private int xDir = 1;
    private int yDir = 1;
    private int zDir = 1;

    void Start(){
        StartCoroutine("randomxDir");
        StartCoroutine("randomyDir");
        StartCoroutine("randomzDir");
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 randomRotation = new Vector3(
        Random.Range(0, 30 * Time.deltaTime) * xDir,
        Random.Range(0, 30 * Time.deltaTime) * yDir,
        Random.Range(0, 30 * Time.deltaTime) * zDir  
        );
        transform.Rotate(randomRotation);         
    }

    public IEnumerator randomxDir(){
        yield return new WaitForSeconds(Random.Range(3, 5));
        xDir = -xDir;
    }
    public IEnumerator randomyDir(){
        yield return new WaitForSeconds(Random.Range(3, 5));
        yDir = -yDir;
    }
    public IEnumerator randomzDir(){
        yield return new WaitForSeconds(Random.Range(3, 5));
        zDir = -zDir;
    }
}}
