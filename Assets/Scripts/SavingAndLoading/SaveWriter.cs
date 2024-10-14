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

    public void buildJSONTree(string activeSceneKey){
        makeFileIfNull();
        loadExistingDataIntoJSONTree(activeSceneKey);
    }

    public void modifyJsonTree(string writeLine){

        modifyJSONSubtree();
        streamFileChanges();
        closeFileStreamAndDeleteDuplicateFile();
        /*The data from the json file should be loaded into a tree
        * saving an object should edit the values in the tree
        * then the tree should be rewritten into the json file
        * by streaming a second file and overriding the first one
        * after the scene is finished editing the tree.
        */
    }

    private void makeFileIfNull(){
        path = Path.Combine(Application.persistentDataPath, "save.json");
        if(!File.Exists(path)){
            using (StreamWriter sw = File.CreateText(path)){
            }
        }
    }

    public void loadExistingDataIntoJSONTree(string activeSceneKey){
        //TODO: locate correct subtree
        //TODO: 
    }

    public void modifyJSONSubtree(){

    }

    public void streamFileChanges(){

    }

    public void closeFileStreamAndDeleteDuplicateFile(){

    }
}
}