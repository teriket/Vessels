using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving{
    [ExecuteInEditMode]
public class ObjectID : MonoBehaviour
{
    [SerializeField]string objectID = null;
    static int nextInt = 0;

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
        string ID = nextInt.ToString();
        objectID = ID;
        nextInt++;
    }

    private bool parentHasID(){
        if(this.transform.parent is null || this.transform.parent.gameObject.GetComponent<ObjectID>() is null){
            return false;
        }
        return true;
    }

    private void addIDComponentToParent(){
        if(this.transform.parent is not null){
            this.transform.parent.gameObject.AddComponent<ObjectID>();
    }}

    public string getID(){
        return objectID;
    }
}
}