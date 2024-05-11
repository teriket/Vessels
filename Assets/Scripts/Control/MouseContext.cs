using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
Author:         Tanner Hunt
Date:           4/21/2024
Version:        0.1.1
Description:    This code sets the current mouse context so that the appropriate actions
                occur when the user uses their mouse i.e. they auto attack in the default
                state, but cast a spell instead when a spell is queued, and the camera
                doesn't move when a menu is open.
ChangeLog:      

*/

namespace Control{
public class MouseContext : MonoBehaviour
{
    public delegate void changedMouseContext(mouseContext newContext);
    public changedMouseContext changedContextEvent;

    public enum mouseContext{
        defaultGamePlay,
        menu
    }
    
    mouseContext context;

    public mouseContext getMouseContext(){
        return context;
    }

    public void setMouseContext(mouseContext newContext){
        context = newContext;
        if(changedContextEvent != null){
        changedContextEvent(newContext);
        }
    }

    void Start(){
        context = mouseContext.defaultGamePlay;
        if(changedContextEvent != null){
        changedContextEvent(context);
        }
    }
}
}