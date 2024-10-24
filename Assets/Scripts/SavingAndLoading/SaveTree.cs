using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveTree : MonoBehaviour
{
    SaveTreeNode root;
    SaveTreeNode curNode;

    public void addNode(SaveTreeNode node){
        if(curNode.getChild(node.key) is not null){
            Debug.Log("Error: File Saving Error.  Tried to write a save value with an already existant key");
            return;
        }
        else{
            curNode.addChild(node.key, node.value);
        }
    }

    public void saveSubtree(SaveTree subtree){
        curNode = root;
        subtree.rerootParallelNode();

        traverseMatchingSubtree(subtree);
        copyRemainingNodes(subtree);
        saveComponentValue(subtree);
    }

    private void copyRemainingNodes(SaveTree subtree){
        SaveTreeNode nextNode = subtree.getParallelNode().getOnlyChild();
        while(nextNode is not null){
            addNode(nextNode);
            curNode = nextNode;
            subtree.setParallelNode(nextNode.key);
        }
    }

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
* Function for debugging purposes, prints each row in the JSON save tree sequentially
*/
    public void printSaveTree(){
        List<SaveTreeNode> currentRow = new List<SaveTreeNode>();
        List<SaveTreeNode> nextRow = new List<SaveTreeNode>();
        currentRow.Add(root);

        while(currentRow.Count > 0){
            string line = "";
            foreach(SaveTreeNode node in currentRow){
                nextRow.AddRange(node.getAllChildren());
                line = line + node.ToString();
            }

            currentRow = nextRow;
            nextRow.Clear();
            print(line);
        }
    }

    public override string ToString(){
    //TODO: convert the subtree into a printable string
    //for the Save file.
    return "asdf";
    }

}
