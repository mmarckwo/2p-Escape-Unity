using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DoorButtonToggleWithObject : NetworkBehaviour
{
    public GameObject offButton;
    public GameObject onButton;

    public bool buttonStatus = false; 

    public GameObject doorToggle;

    [Networked(OnChanged = nameof(onToggleButton))]
    private bool networkStatus { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        networkStatus = !networkStatus;
    }

    static void onToggleButton(Changed<DoorButtonToggleWithObject> changed)
    {
        // set button status to ON.
        changed.Behaviour.buttonStatus = true;

        // turn the button on.
        changed.Behaviour.offButton.SetActive(false);
        changed.Behaviour.onButton.SetActive(true);

        // 
        if (changed.Behaviour.doorToggle ?? false)
        {
            changed.Behaviour.doorToggle.SetActive(true);
        }
    }
}
