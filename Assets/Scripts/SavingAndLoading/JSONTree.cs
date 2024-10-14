using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JSONTree
{

    private static JSONTree instance;
    JSONNode root;

    private JSONTree(){}

    public JSONTree getInstance(){
        if(instance is null){
            instance = new JSONTree();
        }
        return instance;
    }

    public void AddNode(){}

    public JSONNode search(){}

    public override string ToString(){
        //TODO: convert the subtree into a printable string
        //for the json file.
        return "asdf";
    }

}
