using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Debugging{
public abstract class Debugable : MonoBehaviour
{
    public void Start(){
        FindObjectOfType<Debugger>().debugCommand += parseCommand;
    }

    public void parseCommand(string input){
        string[] command = input.Split('.');
        if(command.Length < 2){
            return;
        }
        if(command[0] == this.GetType().Name){
            this.debugAction(command);
        }
    }

    public abstract void debugAction(string[] command);
}
}