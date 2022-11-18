using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StairsVolume : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnStairsCheckPlus))]
    private bool networkStatusPlus { get; set; }
    [Networked(OnChanged = nameof(OnStairsCheckMinus))]
    private bool networkStatusMinus { get; set; }

    private int playerCount = 0;

    public string SceneRef;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        networkStatusPlus = !networkStatusPlus;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        networkStatusMinus = !networkStatusMinus;
    }

    static void OnStairsCheckPlus(Changed<StairsVolume> changed)
    {
        changed.Behaviour.playerCount++;
        // show UI text saying wait for other player. 

        if (changed.Behaviour.playerCount == 2)
        {
            changed.Behaviour.Runner.SetActiveScene(changed.Behaviour.SceneRef);
        }
    }

    static void OnStairsCheckMinus(Changed<StairsVolume> changed)
    {
        changed.Behaviour.playerCount--;
        // clear UI text saying wait for other player. 
    }
}
