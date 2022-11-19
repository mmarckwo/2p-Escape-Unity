using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SceneLoad : NetworkBehaviour
{
    public string SceneRef;
    public GameObject[] buttons;

    [Networked(OnChanged = nameof(OnVolumeCheckPlus))]
    private bool networkStatusPlus { get; set; }
    [Networked(OnChanged = nameof(OnVolumeCheckMinus))]
    private bool networkStatusMinus { get; set; }

    private int playerCount = 0;

    // controls whether playerCount can be changed or not by entering and exiting volume.
    private bool ableToCount = false;

    private bool isAlreadyOverlapping = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
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
                networkStatusPlus = !networkStatusPlus;
            } else
            {
                Debug.Log("not all buttons have been activated.");
                ableToCount = false;
                isAlreadyOverlapping = true;
                // do something with ui here. 
            }
        }
        // no buttons needed.
        else
        {
            // check if both players are in.
            ableToCount = true;
            networkStatusPlus = !networkStatusPlus;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;
        if (!ableToCount) return;

        if (isAlreadyOverlapping)
        {
            isAlreadyOverlapping = false;
            return;
        }

        // remove UI text? same process for setting text on volume enter?

        networkStatusMinus = !networkStatusMinus;
    }

    static void OnVolumeCheckPlus(Changed<SceneLoad> changed)
    {
        changed.Behaviour.playerCount++;

        if (changed.Behaviour.playerCount == 2)
        {
            changed.Behaviour.Runner.SetActiveScene(changed.Behaviour.SceneRef);
        }
    }

    static void OnVolumeCheckMinus(Changed<SceneLoad> changed)
    {
        changed.Behaviour.playerCount--;

        // clear UI text?
    }
}
