using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debugging;

namespace Saving{
    /// <summary>
    /// Calls save on all active objects in the scene.
    /// </summary>
public class LevelSaver : Debugable
{
    SaveWriter saveWriter = SaveWriter.getInstance();

    public void save(){
        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject obj in activeScene.GetRootGameObjects()){
            saveObjectAndAllChildren(obj);
        }
        saveWriter.pushChangesToSaveFile();
    }

    private void saveObjectAndAllChildren(GameObject gameObject){
        for(int i = 0; i < gameObject.transform.childCount; i++){
            saveObjectAndAllChildren(gameObject.transform.GetChild(i).gameObject);
        }
        writeSave(gameObject);
    }

    private void writeSave(GameObject obj){
        if(obj.GetComponent<ISavable>() is null){
            return;
        }
        else{
            saveWriter.modifyJSON(obj.GetComponent<ISavable>().save());
        }
    }

    public override void debugAction(string[] command){
        switch(command[1]){
            case "save":
                save();
                break;
            default:
                print(command[1] + ": invalid input.");
                break;
        }
    }
}
}