using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LockedKeyDoor : NetworkBehaviour
{
    public bool awaitForKey = false;
    [Networked(OnChanged = nameof(awaitTrue))]
    private bool networkStatusTrue { get; set; }
    [Networked(OnChanged = nameof(awaitFalse))]
    private bool networkStatusFalse { get; set; }

    [Networked(OnChanged = nameof(removeDoor))]
    public bool doorStatus { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        other.gameObject.GetComponent<PlayerInventory>().lockedKeyDoor = this.gameObject;

        // set UI text? before hasstateauthority check.
        
        networkStatusTrue = !networkStatusTrue;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        other.gameObject.GetComponent<PlayerInventory>().lockedKeyDoor = null;

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
        changed.Behaviour.transform.parent.gameObject.SetActive(false);
    }
}
