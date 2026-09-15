using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
Author:         Tanner Hunt
Date:           5/23/2024
Version:        0.1.0
Description:    Singleton pattern to create all core game elements: player, UI,
                GameManager, & GrappleHook.  Checks whether any of the core gameobjects
                were removed from the dontdestroyonload list and reinstantiates them.
ChangeLog:      V 0.1.0 -- 
                    --Implemented Code
                    --NYI: text here
                    --Dev time: 0.25 Hours
*/
namespace WorldManagement{
public class BuildCoreGameObjects : MonoBehaviour
{
    public static GameObject gameManager;
    [SerializeField]List<GameObject> buildObjects;
    Transform initTransform;

/// <summary>
/// Keeps only one game manager, and destroys any others in the new scene.  If this
/// is the first time the game manager is loaded, instantiate the core game objects.
/// </summary>
    void Awake()
    {
        SceneManager.sceneUnloaded += onSceneUnloaded;
        if(gameManager == null){
            gameManager = this.transform.gameObject;
            init();
        }
        if(gameManager != this.transform.gameObject){
            Destroy(this.transform.gameObject);
        }
    }

/// <summary>
/// Creates all core game objects the first time the game is loaded.
/// </summary>
    private void init(){
        initTransform = GameObject.FindObjectsOfType<LevelLoader>()[0].transform.GetChild(0).transform;
        foreach(GameObject go in buildObjects){
            GameObject clone = Instantiate(go, initTransform.position, initTransform.rotation);
            clone.name = go.name;
        }
    }

/// <summary>
/// Checks that none of the core game objects were unloaded, then reloads them if they
/// were after old scenes are deloaded.
/// </summary>
/// <param name="current"></param>
    private void onSceneUnloaded(Scene current){
        initTransform = GameObject.FindObjectsOfType<LevelLoader>()[0].transform.GetChild(0).transform;
        foreach(GameObject go in buildObjects){
            GameObject restoreObject = GameObject.Find(go.name);
            restoreObject ??= Instantiate(go); //instantiate if null
            restoreObject.name = go.name;
        }
    }
}
}