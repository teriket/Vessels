using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debugging;
//using System.Text.Json;

namespace Saving{
    /// <summary>
    /// Calls save on all active objects in the scene.
    /// </summary>
public class LevelSaver : MonoBehaviour
{
    //SaveWriter saveWriter = SaveWriter.getInstance();

    void Start(){
        save();
    }

    public void save(){
        Scene activeScene = SceneManager.GetActiveScene();
        //saveWriter.initSaveTree();

        foreach (GameObject obj in activeScene.GetRootGameObjects()){
            saveObjectAndAllChildren(obj);
        }
        //saveWriter.pushChangesToSaveFile();
    }

    private void saveObjectAndAllChildren(GameObject gameObject){
        for(int i = 0; i < gameObject.transform.childCount; i++){
            saveObjectAndAllChildren(gameObject.transform.GetChild(i).gameObject);
        }
        writeChangesToSaveTree(gameObject);
    }

    private void writeChangesToSaveTree(GameObject obj){
        foreach(ISavable component in obj.GetComponents<ISavable>()){
            //saveWriter.buildSaveTree(component.save(), component, obj);
        }
    }

}
}

//string jsonString = JsonUtility.ToJson(obj.GetComponent<ISavable>().save());
