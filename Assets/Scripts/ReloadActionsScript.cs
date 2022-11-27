using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ReloadActionsScript : NetworkBehaviour
{
    private GameObject player1;
    private GameObject player2;

    private BasicSpawner spawner;
    private ItemTrackerScript itemTracker;

    void Start()
    {
        // get the spawner for player refs. 
        spawner = GameObject.FindGameObjectWithTag("BasicSpawner").GetComponent<BasicSpawner>();

        // get the item tracker to know what items to add. 
        itemTracker = GameObject.FindGameObjectWithTag("ItemTracker").GetComponent<ItemTrackerScript>();

        // add expected items and reload the previous scene.
        StartReloadSequence();
    }

    private void StartReloadSequence()
    {
        //player1 = spawner.player1.gameObject;
        //player2 = spawner.player2.gameObject;

        //// get player 1 and 2's inventory scripts.
        //PlayerInventory p1Inventory = player1.GetComponent<PlayerInventory>();
        //PlayerInventory p2Inventory = player2.GetComponent<PlayerInventory>();

        //// if any players are holding items, make them stop holding items.
        //p1Inventory.StopHolding();
        //p2Inventory.StopHolding();

        //// only the host may perform these following actions.
        //if (!Runner.IsServer) return;

        //// clear out player 1's inventory.
        //p1Inventory.ForceAddItem("", 0);
        //p1Inventory.ForceAddItem("", 1);

        //// clear out player 2's inventory.
        //p2Inventory.ForceAddItem("", 0);
        //p2Inventory.ForceAddItem("", 1);

        //// force first and second expected items into player1's inventory.
        //p1Inventory.ForceAddItem(itemTracker.expectedItemA, 0);
        //p1Inventory.ForceAddItem(itemTracker.expectedItemB, 1);

        //// force second and third expected items into player2's inventory.
        //p2Inventory.ForceAddItem(itemTracker.expectedItemC, 0);
        //p2Inventory.ForceAddItem(itemTracker.expectedItemD, 1);

        //// finally, reload the scene we were on.
        //Runner.SetActiveScene(player1.GetComponent<PlayerScript>().currentScene);
    }
}
