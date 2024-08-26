using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDeserializer
{
    private string path;
    private static SaveDeserializer instance;

    private SaveDeserializer(){}

    public SaveDeserializer getInstance(){
        if(instance is null){
            instance = new SaveDeserializer();
        }
        return instance;
    }

    public void rewriteSave(){
        makeFileIfNull();
    }

    private void makeFileIfNull(){
        path = Path.Combine(Application.persistentDataPath, "save.json");
        if(!File.Exists(path)){
            using (StreamWriter sw = File.CreateText(path)){
            }
        }
    }
}
