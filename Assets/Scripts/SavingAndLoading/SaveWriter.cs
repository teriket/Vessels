using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Saving{
    /// <summary>
    /// Modifies the deserialization tree of the save file when save is called.
    /// Rewrites the save file after every object has been saved to the tree.
    /// </summary>
public class SaveWriter
{
    private string path;
    private static SaveWriter instance;

    private SaveWriter(){}

    public static SaveWriter getInstance(){
        if(instance is null){
            instance = new SaveWriter();
        }
        return instance;
    }

    public void pushChangesToSaveFile(){}

    public void buildSaveTree(){
        makeFileIfNull();
    }

    private void makeFileIfNull(){
        path = Path.Combine(Application.persistentDataPath, "save.sav");
        if(!File.Exists(path)){
            using (StreamWriter sw = File.CreateText(path)){
            }
        }
    }

    public void streamFileChanges(){

    }

    public void closeFileStreamAndDeleteDuplicateFile(){

    }
}
}

