using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTreeNode
{
    public string key {get; set;}
    public object value {get; set;}
    private Dictionary<string, SaveTreeNode> children;

    public SaveTreeNode addChild(string newKey, object newValue){
        if(children.ContainsKey(newKey)){
            Debug.Log("Trying to add an object to the save file with an already existant ID.");
            return children[newKey];
        }
        
        SaveTreeNode child = new SaveTreeNode();
        child.key = newKey;
        child.value = newValue;
        children[newKey] = child;

        return children[newKey];
    }

    public SaveTreeNode getChild(string key){
        if(children.ContainsKey(key)){
            return children[key];
        }
        else return null;
    }

    public bool isLeaf(){
        if(children.Count == 0){
            return true;
        }
        return false;
    }

    public SaveTreeNode getOnlyChild(){
        foreach(string key in children.Keys){
            return children[key];
        }
        return null;
    }

    public override string ToString(){
        return key + " : " + value.ToString();
    }

    public List<SaveTreeNode> getAllChildren(){
        List<SaveTreeNode> childrenToReturn = new List<SaveTreeNode>();
        foreach(SaveTreeNode child in children.Values){
            childrenToReturn.Add(child);
        }
        return childrenToReturn;
    }
}
