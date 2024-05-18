using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         
Date:           4/21/2024
Version:        0.1.0
Description:    The methods all States in a statemachine should implement.
ChangeLog:      
*/
public interface IState
{
    public void enter();
    public void execute();
    public void exit();
}
