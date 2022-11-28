using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class LockedKeyDoor : NetworkBehaviour
{
    public bool awaitForKey = false;
    [Networked(OnChanged = nameof(awaitTrue))]
    private bool networkStatusTrue { get; set; }
    [Networked(OnChanged = nameof(awaitFalse))]
    private bool networkStatusFalse { get; set; }

    [Networked(OnChanged = nameof(removeDoor))]
    public bool doorStatus { get; set; }

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

    private GameObject playerRef;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        if (playerRef == null)
        {
            playerRef = other.gameObject;
        }

        other.gameObject.GetComponent<PlayerInventory>().lockedKeyDoor = this.gameObject;

        reasonTextA = "This door ";
        reasonTextB = "needs ";
        reasonTextC = "a key.";

        networkMessage = !networkMessage;
        networkStatusTrue = !networkStatusTrue;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        other.gameObject.GetComponent<PlayerInventory>().lockedKeyDoor = null;

        networkMessageClear = !networkMessageClear;
        networkStatusFalse = !networkStatusFalse;
    }

    static void awaitTrue(Changed<LockedKeyDoor> changed)
    {
        changed.Behaviour.awaitForKey = true;
    }

    static void awaitFalse(Changed<LockedKeyDoor> changed)
    {
        changed.Behaviour.awaitForKey = false;
    }

    static void removeDoor(Changed<LockedKeyDoor> changed)
    {
        // clear key door message, then set to inactive.
        try
        {
            GameObject notifier = changed.Behaviour.playerRef.GetComponent<PlayerInventory>().playerCam.transform.Find("PlayerUICanvas/Notifier").gameObject;
            notifier.SetActive(false);
        } catch (Exception)
        {}

        changed.Behaviour.transform.parent.gameObject.SetActive(false);
    }

    static void OnMessageUpdate(Changed<LockedKeyDoor> changed)
    {
        GameObject notifier = changed.Behaviour.playerRef.GetComponent<PlayerInventory>().playerCam.transform.Find("PlayerUICanvas/Notifier").gameObject;
        notifier.SetActive(true);

        notifier.GetComponent<TextMeshProUGUI>().text = changed.Behaviour.reasonTextA + changed.Behaviour.reasonTextB + changed.Behaviour.reasonTextC;
    }

    static void OnMessageClear(Changed<LockedKeyDoor> changed)
    {
        GameObject notifier = changed.Behaviour.playerRef.GetComponent<PlayerInventory>().playerCam.transform.Find("PlayerUICanvas/Notifier").gameObject;
        notifier.SetActive(false);

        changed.Behaviour.playerRef = null;
    }
}
