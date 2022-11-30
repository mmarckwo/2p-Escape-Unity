using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class StairsVolume : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnStairsCheckPlus))]
    private bool networkStatusPlus { get; set; }
    [Networked(OnChanged = nameof(OnStairsCheckMinus))]
    private bool networkStatusMinus { get; set; }

    private int playerCount = 0;

    public string SceneRef;

    private GameObject playerRef;

    [Networked(OnChanged = nameof(OnMessageUpdate))]
    private bool networkMessage { get; set; }

    [Networked(OnChanged = nameof(OnMessageClear))]
    private bool networkMessageClear { get; set; }

    [Networked]
    private string reasonTextA { get; set; }
    [Networked]
    private string reasonTextB { get; set; }
    [Networked]
    private string reasonTextC { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        if (playerRef == null)
        {
            playerRef = other.gameObject;
        }

        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        reasonTextA = "Wait for ";
        reasonTextB = "the other ";
        reasonTextC = "player.";

        networkMessage = !networkMessage;

        networkStatusPlus = !networkStatusPlus;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        networkMessageClear = !networkMessageClear;
        networkStatusMinus = !networkStatusMinus;
    }

    static void OnMessageUpdate(Changed<StairsVolume> changed)
    {
        try
        {
            GameObject notifier = changed.Behaviour.playerRef.GetComponent<PlayerInventory>().playerCam.transform.Find("PlayerUICanvas/Notifier").gameObject;
            notifier.SetActive(true);

            notifier.GetComponent<TextMeshProUGUI>().text = changed.Behaviour.reasonTextA + changed.Behaviour.reasonTextB + changed.Behaviour.reasonTextC;
        } catch (Exception) 
        {}
        
    }

    static void OnMessageClear(Changed<StairsVolume> changed)
    {
        try
        {
            GameObject notifier = changed.Behaviour.playerRef.GetComponent<PlayerInventory>().playerCam.transform.Find("PlayerUICanvas/Notifier").gameObject;
            notifier.SetActive(false);
        } catch (Exception) 
        {}

        changed.Behaviour.playerRef = null;
    }

    static void OnStairsCheckPlus(Changed<StairsVolume> changed)
    {
        changed.Behaviour.playerCount++;

        try
        {
            GameObject notifier = changed.Behaviour.playerRef.GetComponent<PlayerInventory>().playerCam.transform.Find("PlayerUICanvas/Notifier").gameObject;
            notifier.SetActive(false);
        } catch (Exception)
        {}

        if (changed.Behaviour.playerCount == 2)
        {
            changed.Behaviour.Runner.SetActiveScene(changed.Behaviour.SceneRef);
        }
    }

    static void OnStairsCheckMinus(Changed<StairsVolume> changed)
    {
        changed.Behaviour.playerCount--;
    }
}
