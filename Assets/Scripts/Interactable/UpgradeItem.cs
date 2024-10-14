using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;

namespace Environment{
public class UpgradeItem : MonoBehaviour, ISavable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public object save(){
        return this.transform.position;
    }
    public void load(){}
}
}