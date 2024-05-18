using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
Author:         Tanner Hunt
Date:           5/15/2024
Version:        0.1.0
Description:    loads in the next level and places the character at the appropriate
                spawn point.
ChangeLog:      V 0.1.0 -- 5/15/2024
                    --NYI: Fix bug where jumping through  portal doesn't move player
                    --NYI: Fix bug where levelLoaders in next level don't work
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
            print("collision");
            player = collider.gameObject;
            StartCoroutine("loadSceneAsync");
        }
    }

    IEnumerator loadSceneAsync(){
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndexToTransitionTo, LoadSceneMode.Additive);
        while(!asyncLoad.isDone){
            yield return null;
        }

        //find the correct spawn point to load into, then transfer player there
        foreach(LevelLoader levelLoader in GameObject.FindObjectsOfType<LevelLoader>()){
            if(levelLoader.gameObject.scene == this.gameObject.scene){
                continue;
            }

            if(levelLoader.thisLoadersNumber == levelLoaderIndexToTeleportTo){
                print("found it! " + levelLoader.transform.GetChild(0).position);
                player.transform.parent.position = levelLoader.transform.GetChild(0).position;
            }
        }

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(this.gameObject.scene.buildIndex);

    }
}}
