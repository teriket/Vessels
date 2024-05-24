using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
Author:         Tanner Hunt
Date:           5/23/2024
Version:        0.2.0
Description:    loads in the next level and places the character at the appropriate
                spawn point.
ChangeLog:      V 0.2.0 -- 5/23/2024
                    --Now moved the player to the last level loader found during
                    the discover phase if none of the level loaders have the appropriate
                    ID to move the player to.

*/
namespace WorldManagement{
public class LevelLoader : MonoBehaviour
{
    [SerializeField]int sceneIndexToTransitionTo;
    [SerializeField]string LevelBeingLoaded;
    [SerializeField]int thisLoadersNumber;
    [SerializeField]int levelLoaderIndexToTeleportTo;
    private int levelLoaderIDToTeleportTo;
    private Vector3 spawnPoint;
    GameObject player;

    void Start(){
        spawnPoint = transform.GetChild(0).position;
    }

    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag == "Player"){
            player = collider.gameObject;
            StartCoroutine("loadSceneAsync");
        }
    }

    IEnumerator loadSceneAsync(){
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndexToTransitionTo, LoadSceneMode.Additive);
        while(!asyncLoad.isDone){
            yield return null;
        }

        LevelLoader lastLevelLoader = null;
        //find the correct spawn point to load into, then transfer player there
        foreach(LevelLoader levelLoader in GameObject.FindObjectsOfType<LevelLoader>()){
            if(levelLoader.gameObject.scene == this.gameObject.scene){
                continue;
            }

            if(levelLoader.thisLoadersNumber == levelLoaderIndexToTeleportTo){
                player.transform.parent.position = levelLoader.transform.GetChild(0).position;
            }
            lastLevelLoader = levelLoader;
        }

        //if no level loader with the correct ID is found
        Debug.LogError("Did not find a levelLoader ID when loading into the level");
        player.transform.parent.position = lastLevelLoader.transform.GetChild(0).transform.position;

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(this.gameObject.scene.buildIndex);

    }
}}
