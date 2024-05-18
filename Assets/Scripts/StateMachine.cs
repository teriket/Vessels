using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         
Date:           4/21/2024
Version:        0.1.0
Description:    Manages the state of a script.  To use, a monobehaviour should declare
                a variable StateMachine currentState = new StateMachine();  In Start(),
                the method should call currentState.changeState(new theNewState(this)).
                In update, the script should call currentState.Update();, or whatever
                timeframe the state needs to be called.
ChangeLog:      
*/
public class StateMachine
{

    IState currentState;

    public void changeState(IState newState){
        if(currentState != null){
            currentState.exit();
        }
        currentState = newState;
        currentState.enter();
    }

    public void update()
    {
        if(currentState != null){
            currentState.execute();
        }
    }
}
