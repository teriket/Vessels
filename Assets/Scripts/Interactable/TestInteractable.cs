using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable{
public class TestInteractable : MonoBehaviour, IInteractable
{
    public void onInteract(){
        Debug.Log("interacting");
    }
}}
