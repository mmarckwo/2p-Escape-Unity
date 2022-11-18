using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DoorButtonToggle : NetworkBehaviour
{
    public GameObject offButton;
    public GameObject onButton;

    public bool buttonStatus = false;

    [Networked(OnChanged = nameof(onToggleButton))]
    private bool networkStatus { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return; // comment this out to toggle with thrown items.
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        networkStatus = !networkStatus;
    }

    static void onToggleButton(Changed<DoorButtonToggle> changed)
    {
        changed.Behaviour.buttonStatus = !changed.Behaviour.buttonStatus;

        if (changed.Behaviour.buttonStatus == false)
        {
            changed.Behaviour.offButton.SetActive(true);
            changed.Behaviour.onButton.SetActive(false);
        } else
        {
            changed.Behaviour.offButton.SetActive(false);
            changed.Behaviour.onButton.SetActive(true);
        }
    }
}
