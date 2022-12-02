using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ObjectToggler : NetworkBehaviour
{
    public GameObject offButton;
    public GameObject onButton;

    public bool buttonStatus = false;

    public GameObject[] objectsToggleA;
    public GameObject[] objectsToggleB;

    [Networked(OnChanged = nameof(onToggleButton))]
    private bool networkStatus { get; set; }

    [Header("Sounds")]
    public AudioSource clickSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return; // comment this out to toggle with thrown items.
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        networkStatus = !networkStatus;
    }

    static void onToggleButton(Changed<ObjectToggler> changed)
    {
        changed.Behaviour.buttonStatus = !changed.Behaviour.buttonStatus;

        if (changed.Behaviour.buttonStatus == true)
        {
            changed.Behaviour.offButton.SetActive(true);
            changed.Behaviour.onButton.SetActive(false);

            // if there is a door to toggle, then set it to inactive.
            //if (changed.Behaviour.doorToggle ?? false)
            //{
            //    changed.Behaviour.doorToggle.SetActive(false);
            //}

            // turn off objects in set A.
            foreach (GameObject objectItemA in changed.Behaviour.objectsToggleA)
            {
                objectItemA.SetActive(false);
            }

            // turn on objects in set B.
            foreach (GameObject objectItemB in changed.Behaviour.objectsToggleB)
            {
                objectItemB.SetActive(true);
            }
        }
        else
        {
            changed.Behaviour.offButton.SetActive(false);
            changed.Behaviour.onButton.SetActive(true);

            // if there is a door to toggle, then set it to active.
            //if (changed.Behaviour.doorToggle ?? false)
            //{
            //    changed.Behaviour.doorToggle.SetActive(true);
            //}

            // turn on objects in set A.
            foreach (GameObject objectItemA in changed.Behaviour.objectsToggleA)
            {
                objectItemA.SetActive(true);
            }

            // turn off objects in set B.
            foreach (GameObject objectItemB in changed.Behaviour.objectsToggleB)
            {
                objectItemB.SetActive(false);
            }
        }

        changed.Behaviour.clickSound.Play();
    }
}
