using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
    GameObject go;
    go = this.gameObject;
    {
        while(go.transform.parent is not null){
            print(go.name);
            go = go.transform.parent.gameObject;
        }
    }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
