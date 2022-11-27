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

    void Start()
    {
        //GameObject[] itemTrackerDuplicates = GameObject.FindGameObjectsWithTag("ItemTracker");

        //if (itemTrackerDuplicates.Length > 1)
        //{
        //    NetworkRunner runner = GameObject.FindGameObjectWithTag("BasicSpawner").GetComponent<NetworkRunner>();
        //    runner.Despawn(itemTrackerDuplicates[1].gameObject.GetComponent<NetworkObject>());
        //}

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

        // find expected items for subsequent scenes.
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

}
