using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving{
    [ExecuteInEditMode]
public class ObjectID : MonoBehaviour
{
    [SerializeField]string objectID = null;

    void Awake(){
        if(!isvalidID()){
            assignID();
        }
        if(!parentHasID()){
            addIDComponentToParent();
        }
    }

    private bool isvalidID(){
        if(objectID is null){
            return false;
        }
        return true;
    }

    private void assignID(){
        string ID = "";
        for(int i = 0; i < 16; i++){

        }
        objectID = ID;
    }

    private bool parentHasID(){
        if(this.transform.parent.gameObject.GetComponent<ObjectID>() is null){
            return false;
        }
        return true;
    }

    private void addIDComponentToParent(){
        this.transform.parent.gameObject.AddComponent<ObjectID>();
    }

    public string getID(){
        return objectID;
    }
}
}