using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
/**
Author:         Tanner Hunt
Date:           4/21/2024
Version:        0.1.2
Description:    This code toggles on and off the escape menu if the player presses the escape key
ChangeLog:      

*/

namespace UI{
public class EscapeMenu : MonoBehaviour
{
    GameObject escapeMenuCanvas;        //the menu to be opened
    bool isActive = false;              //whether the menu should be open or closed
    MouseContext playerMouseContext;

/// <summary>
/// Cache a reference to the escape menu, then close the menu.
/// </summary>
    void Start(){
        escapeMenuCanvas = transform.Find("Escape Menu").gameObject;
        escapeMenuCanvas.SetActive(isActive);
        playerMouseContext = GameObject.Find("Character").GetComponent<MouseContext>();
    }

/// <summary>
/// Listens for input into the escape key.  Opens and closes the menu each time the
/// key is pressed.
/// </summary>
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            ToggleMenu();
        }
    }

    public void ToggleMenu(){
        isActive = !isActive;
        escapeMenuCanvas.SetActive(isActive);
        if(isActive){
            playerMouseContext.setMouseContext(MouseContext.mouseContext.menu);
        }
        else{
            playerMouseContext.setMouseContext(MouseContext.mouseContext.defaultGamePlay);
        }
    }
}
}