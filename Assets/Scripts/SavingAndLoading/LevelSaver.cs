using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debugging;
using System.Text.Json;

namespace Saving{
    /// <summary>
    /// Calls save on all active objects in the scene.
    /// </summary>
public class LevelSaver : Debugable
{
    SaveWriter saveWriter = SaveWriter.getInstance();

    public void save(){
        Scene activeScene = SceneManager.GetActiveScene();
        saveWriter.buildJSONTree(activeScene.name);
        foreach (GameObject obj in activeScene.GetRootGameObjects()){
            saveObjectAndAllChildren(obj);
        }
        saveWriter.pushChangesToSaveFile();
    }

    private void saveObjectAndAllChildren(GameObject gameObject){
        for(int i = 0; i < gameObject.transform.childCount; i++){
            saveObjectAndAllChildren(gameObject.transform.GetChild(i).gameObject);
        }
        writeChangesToJsonTree(gameObject);
    }

//TODO: an object may have multiple isavable scripts.  ITerate over each isavable.
    private void writeChangesToJsonTree(GameObject obj){
        if(obj.GetComponent<ISavable>() is null){
            return;
        }
        else{
            string jsonString = JsonUtility.ToJson(obj.GetComponent<ISavable>().save());
            saveWriter.modifyJsonTree(jsonString);
        }
    }

    public override void debugAction(string[] command){
        switch(command[1]){
            case "save":
                print("saving");
                save();
                break;
            default:
                print(command[1] + ": invalid input.");
                break;
        }
    }
}
}