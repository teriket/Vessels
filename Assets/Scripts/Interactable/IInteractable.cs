using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           4/20/2024
Version:        0.1.0
Description:    Interface for interactable objects you press a key to interact with.  All
                behaviors are executed once in the onInteract() function.
ChangeLog:      
*/
namespace Interactable{
public interface IInteractable
{
    public void onInteract();
}}
