using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONNode
{
    public string key {get; set;}
    public string value {get; set;}
    private Dictionary<string, JSONNode> children;

    public JSONNode addChild(string newKey, string newValue){
        if(children.ContainsKey(newKey)){
            Debug.Log("Trying to add an object to the save file with an already existant ID.");
            return children[newKey];
        }
        
        JSONNode child = new JSONNode();
        child.key = newKey;
        child.value = newValue;
        children[newKey] = child;

        return children[newKey];
    }

    public JSONNode getChild(string key){
        if(children.ContainsKey(key)){
            return chcildren[key];
        }
        else return null;
    }
}
