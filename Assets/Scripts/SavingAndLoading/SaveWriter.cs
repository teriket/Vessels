using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving{
    /// <summary>
    /// Modifies the deserialization tree of the save file when save is called.
    /// Rewrites the save file after every object has been saved to the tree.
    /// </summary>
public class SaveWriter
{
    private static SaveWriter instance;

    private SaveWriter(){}

    public static SaveWriter getInstance(){
        if(instance is null){
            instance = new SaveWriter();
        }
        return instance;
    }

    public void modifyJSON(string writeLine){
        modifyDeserializedFile();
    }

    private void modifyDeserializedFile(){
        /*The data from the json file should be loaded into a tree
        * saving an object should edit the values in the tree
        * then the tree should be rewritten into the json file
        * by streaming a second file and overriding the first one
        * after the scene is finished editing the tree.
        */
    }

    public void pushChangesToSaveFile(){}
}
}