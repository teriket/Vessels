using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTreeNode
{
    public string key {get; set;}
    public object value {get; set;}
    private Dictionary<string, SaveTreeNode> children = new Dictionary<string, SaveTreeNode>();

    public SaveTreeNode addChild(string newKey, object newValue){
        if(children.ContainsKey(newKey)){
            return children[newKey];
        }  

        SaveTreeNode child = new SaveTreeNode();

        child.key = newKey;
        child.value = newValue;

        children.Add(newKey, child);
        return children[newKey];
    }

    public SaveTreeNode addChild(SaveTreeNode newNode){
        return this.addChild(newNode.key, newNode.value);
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
        if(this.value is null){
            return key + ": ";
        }
        return key + " : " + value.ToString();
    }

    public List<SaveTreeNode> getAllChildren(){
        List<SaveTreeNode> childrenToReturn = new List<SaveTreeNode>();
        foreach(SaveTreeNode child in children.Values){
            childrenToReturn.Add(child);
        }
        return childrenToReturn;
    }

    public void printAllDownstream(){
        Debug.Log(this.ToString());
        foreach(string child in children.Keys){
            children[child].printAllDownstream();
        }

    }
}
