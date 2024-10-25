using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Saving{
public class SaveTree
{
    SaveTreeNode root;
    SaveTreeNode curNode;

    public SaveTree(string key){
        SaveTreeNode newRootNode = new SaveTreeNode();
        newRootNode.key = key;
        newRootNode.value = null;
        this.root = newRootNode;
    }

    public void addNode(SaveTreeNode node){
        if(curNode.getChild(node.key) is not null){
            curNode = curNode.getChild(node.key);
            return;
        }
        else{
            curNode = curNode.addChild(node.key, node.value);
        }
    }

/**
* builds a savetree based on an Isavable component to be used in parallel with
* the primary tree.
*/
    public void buildSubtree(object saveData, ISavable component, GameObject componentParent){
        curNode = root;
        addSceneToTree();
        addGameObjectPathToTree(componentParent);
        addObjectDataToTree(saveData, component);
    }

/**
*
*/
    private void addSceneToTree(){
        string sceneID = SceneManager.GetActiveScene().name;
        
        SaveTreeNode sceneNode = new SaveTreeNode();
        sceneNode.key = sceneID;

        addNode(sceneNode);

    }

/***/
    private void addObjectDataToTree(object saveData, ISavable component){

        SaveTreeNode leafNode = new SaveTreeNode();
        leafNode.key = component.GetType().Name;
        leafNode.value = saveData;
        addNode(leafNode);
    }
/**
*
*/
    private void addGameObjectPathToTree(GameObject componentParent){
        Stack<SaveTreeNode> nodeStack = new Stack<SaveTreeNode>();

        GameObject go = componentParent;
        while(go is not null){
            SaveTreeNode newNode = new SaveTreeNode();
            newNode.key = go.name;
            nodeStack.Push(newNode);

            if(go.transform.parent is not null){
                go = go.transform.parent.gameObject;
            }
            else{ go = null; }
        }

        foreach(SaveTreeNode node in nodeStack){
            addNode(node);
        }
    }

/**
* copies a parallel tree into the primary tree
*/
    public void saveSubtree(SaveTree subtree){
        curNode = root;
        subtree.rerootParallelNode();

        traverseMatchingSubtree(subtree);
        copyRemainingNodes(subtree);
        saveComponentValue(subtree);
    }

/**
* Updates the primary tree with any missing nodes in the parallel subtree.
*/
    private void copyRemainingNodes(SaveTree subtree){
        SaveTreeNode nextNode = subtree.getParallelNode().getOnlyChild();
        while(nextNode is not null){
            addNode(nextNode);
            subtree.setParallelNode(nextNode.key);
            nextNode = subtree.getParallelNode().getOnlyChild();
        }
    }

/**
* updates the value of a leaf node given a parallel tree.
*/
    private void saveComponentValue(SaveTree subtree){
        curNode.value = getParallelNode().value;
    }

/**
* ISavable objects in the scene send a SaveTree made from the component to object to
* scene to game.  Search the current SaveTree for an identical tree and return a
* leaf node if an identical subtree exists.
*/
    public SaveTreeNode searchForLeaf(SaveTree subtree){
        curNode = root;
        subtree.rerootParallelNode();
        
        while(subtreesMatch(subtree)){
            if (curNode.isLeaf() && subtree.getParallelNode().isLeaf()){
                return curNode;
            }
            string nextNode = subtree.getParallelNode().getOnlyChild().key;
            curNode = curNode.getChild(nextNode);
            subtree.setParallelNode(nextNode);
        }
        
        return null;
    }

/**
* test if the current node is identically the parallel node
*/
private bool subtreesMatch(SaveTree subtree){
    string nextNode = subtree.getParallelNode().getOnlyChild().key;
    if(nextNode is null){
        return false;
    }
    if(curNode.getChild(nextNode) is not null){
        return true;
    }
    return false;
}

/**
* A function used when two SaveTrees are traversed in parallel.  Returns the
* current node in the second tree, which is queried by the primary tree.
*/
    protected SaveTreeNode getParallelNode(){
        return curNode;
    }

/**
* A function used when two SaveTrees are traversed in parallel.  Moves the 
* current node in the second tree to the next value in lockstep with the first
* tree.
*/
    protected void setParallelNode(string key){
        curNode = curNode.getChild(key);
    }

/**
* A function used when two SaveTrees are traversed in parallel.  Moves the current
* node to the root node.
*/
    protected void rerootParallelNode(){
        curNode = root;
    }

/**
* A function used to move down two identical subtrees until no more matching
* SaveTreeNodes are found.
*/
    private void traverseMatchingSubtree(SaveTree subtree){
        while(subtreesMatch(subtree)){
            string nextNode = subtree.getParallelNode().getOnlyChild().key;
            curNode = curNode.getChild(nextNode);
            subtree.setParallelNode(nextNode);
        }
    }

/**
* Function for debugging purposes, prints each row in the save tree sequentially
*/
    public void printSaveTree(){
        List<SaveTreeNode> unvisitedNodes = new List<SaveTreeNode>();
        unvisitedNodes.Add(root);

        Debug.Log("--------------------------------------------------------------\n");
/*         while(unvisitedNodes.Count > 0){
            Debug.Log(unvisitedNodes[0].ToString());
            foreach(SaveTreeNode newNode in unvisitedNodes[0].getAllChildren()){
                unvisitedNodes.Add(newNode);
            }
            unvisitedNodes.RemoveAt(0);
        } */
        root.printAllDownstream();

        Debug.Log("--------------------------------------------------------------\n");

    }

    public override string ToString(){
    //TODO: convert the subtree into a printable string
    //for the Save file.
    return "asdf";
    }

}
}