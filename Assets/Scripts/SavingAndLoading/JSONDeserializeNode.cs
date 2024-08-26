using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONDeserializeNode
{
    public string key {get; set;}
    public string value {get; set;}
    private Dictionary<string, JSONDeserializeNode> children;

    public JSONDeserializeNode addChild(string newKey, string newValue){
        if(children.ContainsKey(newKey)){
            Debug.Log("Trying to add an object to the save file with an already existant ID.");
            return children[newKey];
        }
        
        JSONDeserializeNode child = new JSONDeserializeNode();
        child.key = newKey;
        child.value = newValue;
        children[newKey] = child;

        return children[newKey];
    }
}
