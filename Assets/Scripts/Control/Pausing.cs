using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           4/22/2024
Version:        0.1.0
Description:    Pauses physics simulations when the mouse context is changed
ChangeLog:      V 0.1.0 -- 4/23/2024
                    --Changes the time scale based on the mouse context
                    --NYI: Send a message to other scripts to cease their activity?
                    --Dev time: 0.25 Hours
*/
namespace Control{
public class Pausing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MouseContext>().changedContextEvent += pauseGame;
    }

    // Update is called once per frame
    public void pauseGame(MouseContext.mouseContext context){
        switch(context){
            case MouseContext.mouseContext.menu:
                Time.timeScale = 0;
                break;
            case MouseContext.mouseContext.defaultGamePlay:
            default:
                Time.timeScale = 1;
                break;
        }
    }
}
}