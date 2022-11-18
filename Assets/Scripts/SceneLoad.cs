using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SceneLoad : NetworkBehaviour
{
    public string SceneRef;
    public GameObject[] buttons;

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
                Runner.SetActiveScene(SceneRef);
            } else
            {
                Debug.Log("not all buttons have been activated.");
                // do something with ui here. 
            }
        }
        // no buttons needed.
        else
        {
            Runner.SetActiveScene(SceneRef);
        }
    }
}
