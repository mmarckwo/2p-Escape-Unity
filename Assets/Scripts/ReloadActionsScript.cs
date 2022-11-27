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

    private bool sequenceStart = false;

    private bool taskAwaiter = false;

    TickTimer waitToReloadTimer = TickTimer.None;

    void Start()
    {
        // get the spawner for player refs. 
        spawner = GameObject.FindGameObjectWithTag("BasicSpawner").GetComponent<BasicSpawner>();

        // get the item tracker to know what items to add. 
        itemTracker = GameObject.FindGameObjectWithTag("ItemTracker").GetComponent<ItemTrackerScript>();

        // wait for a bit.
        waitToReloadTimer = TickTimer.CreateFromSeconds(spawner.GetComponent<NetworkRunner>(), 0.2f);
    }

    public override void FixedUpdateNetwork()
    {
        if (waitToReloadTimer.ExpiredOrNotRunning(spawner.GetComponent<NetworkRunner>()))
        {
            // only call this once.
            if (sequenceStart) return;
            sequenceStart = true;

            StartCoroutine(StartReloadSequence());
        }
    }

    IEnumerator StartReloadSequence()
    {
        // only the host may perform these following actions.
        if (!spawner.GetComponent<NetworkRunner>().IsServer) yield break;

        player1 = spawner.player1.gameObject;
        player2 = spawner.player2.gameObject;

        // get player 1 and 2's inventory scripts.
        PlayerInventory p1Inventory = player1.GetComponent<PlayerInventory>();
        PlayerInventory p2Inventory = player2.GetComponent<PlayerInventory>();

        // if any players are holding items, make them stop holding items.
        p1Inventory.StopHolding();
        p2Inventory.StopHolding(); // need to get this to run for the client too.

        // vVv await each force add so that the networked inventory bool can update and add. vVv
        //     when a task is done, wait for the next one to finish.
        //     also, interspersed waiting for consistency over the network.

        // force first and second expected items into player 1's inventory. 
        while (taskAwaiter == false)
        {
            taskAwaiter = p1Inventory.ForceAddItem(itemTracker.expectedItemA, 0);
            yield return null;
        }
        taskAwaiter = false;

        yield return new WaitForSeconds(1.5f);

        while (taskAwaiter == false)
        {
            taskAwaiter = p1Inventory.ForceAddItem(itemTracker.expectedItemB, 1);
            yield return null;
        }
        taskAwaiter = false;

        yield return new WaitForSeconds(1.5f);

        // force second and third expected items into player 2's inventory.
        while (taskAwaiter == false)
        {
            taskAwaiter = p2Inventory.ForceAddItem(itemTracker.expectedItemC, 0);
            yield return null;
        }
        taskAwaiter = false;

        yield return new WaitForSeconds(1.5f);

        while (taskAwaiter == false)
        {
            taskAwaiter = p2Inventory.ForceAddItem(itemTracker.expectedItemD, 1);
            yield return null;
        }
        taskAwaiter = false;

        yield return new WaitForSeconds(1.5f);

        // finally, reload the scene we were on.
        spawner.GetComponent<NetworkRunner>().SetActiveScene(player1.GetComponent<PlayerScript>().currentScene);
    }
}
