using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class SceneLoad : NetworkBehaviour
{
    public string SceneRef;
    public GameObject[] buttons;

    [Networked(OnChanged = nameof(OnVolumeCheckPlus))]
    private bool networkStatusPlus { get; set; }
    [Networked(OnChanged = nameof(OnVolumeCheckMinus))]
    private bool networkStatusMinus { get; set; }

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

    private int playerCount = 0;

    // controls whether playerCount can be changed or not by entering and exiting volume.
    private bool ableToCount = false;

    private bool isAlreadyOverlapping = false;

    private GameObject playerRef;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        if (playerRef == null)
        {
            playerRef = other.gameObject;
        } 
        
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        // if there are buttons that need to be pressed, then check to see if they're all pressed.
        if (buttons.Length != 0)
        {
            // condition.
            int count = buttons.Length;
            // presssed buttons.
            int countChecker = 0;

            // check each button to see if it is pressed.
            foreach (GameObject button in buttons)
            {
                if (button.GetComponent<DoorButtonToggle>().buttonStatus) countChecker++;
            }

            // if all buttons have been been pressed, switch scenes.
            if (countChecker == count)
            {
                // check if both players are in.
                ableToCount = true;

                reasonTextA = "Wait for ";
                reasonTextB = "the other ";
                reasonTextC = "player.";

                networkMessage = !networkMessage;

                networkStatusPlus = !networkStatusPlus;
            } else
            {
                reasonTextA = "Not all buttons ";
                reasonTextB = "have been ";
                reasonTextC = "activated.";
                ableToCount = false;
                isAlreadyOverlapping = true;

                // NETWORKED EVENT
                networkMessage = !networkMessage;
            }
        }
        // no buttons needed.
        else
        {
            // check if both players are in.
            ableToCount = true;

            reasonTextA = "Wait for ";
            reasonTextB = "the other ";
            reasonTextC = "player.";

            networkMessage = !networkMessage;

            networkStatusPlus = !networkStatusPlus;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        networkMessageClear = !networkMessageClear;

        if (!ableToCount) return;

        if (isAlreadyOverlapping)
        {
            isAlreadyOverlapping = false;
            return;
        }

        networkStatusMinus = !networkStatusMinus;
    }

    static void OnMessageUpdate(Changed<SceneLoad> changed)
    {
        try
        {
            GameObject notifier = changed.Behaviour.playerRef.GetComponent<PlayerInventory>().playerCam.transform.Find("PlayerUICanvas/Notifier").gameObject;
            notifier.SetActive(true);

            notifier.GetComponent<TextMeshProUGUI>().text = changed.Behaviour.reasonTextA + changed.Behaviour.reasonTextB + changed.Behaviour.reasonTextC;
        } catch (Exception)
        {}
        
    }

    static void OnMessageClear(Changed<SceneLoad> changed)
    {
        try
        {
            GameObject notifier = changed.Behaviour.playerRef.GetComponent<PlayerInventory>().playerCam.transform.Find("PlayerUICanvas/Notifier").gameObject;
            notifier.SetActive(false);
        } catch (Exception) 
        {}
        
        changed.Behaviour.playerRef = null;
    }

    static void OnVolumeCheckPlus(Changed<SceneLoad> changed)
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

    static void OnVolumeCheckMinus(Changed<SceneLoad> changed)
    {
        changed.Behaviour.playerCount--;
    }
}
