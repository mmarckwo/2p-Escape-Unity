using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemTrackerScript : NetworkBehaviour, ISceneLoadDone
{
    public string expectedItemA;
    public string expectedItemB;
    public string expectedItemC;
    public string expectedItemD;

    public override void Spawned()
    {

        // if the starting room is reloaded, then only keep track of the first item tracker that spawned when the game first started.
        GameObject[] itemTrackerDuplicates = GameObject.FindGameObjectsWithTag("ItemTracker");
        if (itemTrackerDuplicates.Length > 1)
        {
            return; 
        }

        DontDestroyOnLoad(this);

        GameObject expectedItemList = GameObject.FindGameObjectWithTag("ExpectedItems");

        // find expected items for the first scene.
        if (expectedItemList)
        {
            ExpectedItems expItems = expectedItemList.GetComponent<ExpectedItems>();

            expectedItemA = expItems.itemA;
            expectedItemB = expItems.itemB;
            expectedItemC = expItems.itemC;
            expectedItemD = expItems.itemD;
        }
        else
        {};
    }

    public void SceneLoadDone()
    {
        GameObject expectedItemList = GameObject.FindGameObjectWithTag("ExpectedItems");
        Debug.Log("what");
        Debug.Log(this.gameObject);

        // find expected items for subsequent scenes.
        if (expectedItemList)
        {
            Debug.Log("found");
            ExpectedItems expItems = expectedItemList.GetComponent<ExpectedItems>();

            expectedItemA = expItems.itemA;
            expectedItemB = expItems.itemB;
            expectedItemC = expItems.itemC;
            expectedItemD = expItems.itemD;
        }
        else
        { Debug.Log("not found"); };
    }

}
