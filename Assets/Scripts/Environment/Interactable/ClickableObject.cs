using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/7/2024
Version:        0.1.0
Description:    Makes an interactable object glow if the player character is within range.
                If the player looks at the object and presses the interact button, it will
                activate an IInteractable component on the game object.
ChangeLog:      V 0.1.0 -- 5/7/2024
                    --Object now glows if player approaches
                    --press the f key if the player is looking at the object to interact
                    --NYI: Generate a tooltip to press button and change mouse cursor
                    --Dev time: 0.25 Hours
*/
namespace Interactable{
public class ClickableObject : MonoBehaviour
{
    IInteractable behavior;
    [SerializeField]float maxInteractDistance;
    [SerializeField]float interactableAngle;
    GameObject playerCamera;
    GameObject player;
    ParticleSystem ps;
    private Vector3 cameraPlayerLine;
    private Vector3 objectPlayerLine;

    // Start is called before the first frame update
    void Start()
    {
        behavior = GetComponent<IInteractable>();
        player = GameObject.Find("Your Character").transform.GetChild(3).gameObject;
        playerCamera = GameObject.Find("Your Character").transform.GetChild(1).gameObject;
        ps = transform.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
    var em = ps.emission;
        if(Vector3.Distance(player.transform.position, transform.position) < maxInteractDistance){
            cameraPlayerLine = new Vector3(
                playerCamera.transform.position.x - player.transform.position.x,
                playerCamera.transform.position.y - player.transform.position.y,
                playerCamera.transform.position.z - player.transform.position.z                
            ) * -1;
            objectPlayerLine = new Vector3(
                transform.position.x - player.transform.position.x,
                transform.position.y - player.transform.position.y,
                transform.position.z - player.transform.position.z
            ) * -1;
            em.enabled = true;
            if(Mathf.Abs(Vector3.Angle(cameraPlayerLine, objectPlayerLine) - 180) < interactableAngle
                && Input.GetButtonUp("Interact")){
                behavior.onInteract();
            }
        }
        else em.enabled = false;
    }
}}
