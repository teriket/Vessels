using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           5/15/2024
Version:        0.1.0
Description:    Creates a targeting point where the grapple hook should hit if
                the surface is in range of the grapple hook and it is not cooling
                down.
ChangeLog:      V 0.1.0 -- 5/15/2024
                    --Reticle is now a sphere that appears at the target location
                    --reticle now disappears while the grapple hook is recharging
*/
namespace Abilities{
public class TargetReticle : MonoBehaviour
{
    Transform cameraController;
    Transform player;
    [SerializeField]GameObject reticle;
    GrappleHook grappleHook;
    int layerMask = (1 << 16);


    void Start()
    {
        grappleHook = GetComponent<GrappleHook>();
        cameraController = GameObject.Find("Your Character").transform.GetChild(1);
        player = cameraController.transform.parent.GetChild(3);
        reticle = Instantiate(reticle, transform.position, transform.rotation);
        reticle.transform.SetParent(grappleHook.transform);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        RaycastHit hit;
        Physics.Raycast(player.position, cameraController.transform.forward, out hit, Mathf.Infinity, ~layerMask);
        reticle.transform.position = hit.point;
        if(Vector3.Distance(player.position, reticle.transform.position) < grappleHook.maxHookDistane && !grappleHook.isCoolingDown){
            reticle.GetComponent<MeshRenderer>().enabled = true;
        }
        else reticle.GetComponent<MeshRenderer>().enabled = false;
    }
}}
