using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Teleporter_USE : NetworkBehaviour
{
    public GameObject portal;

    private GameObject portalInstanceRef;

    private GameObject placePortalObj;
    private GameObject readyPortalObj;

    private NetworkId portalId { get; set; }

    private GameObject player;

    private bool foundPortal = false;

    [Networked(OnChanged = nameof(onTogglePortal))]
    public bool networkStatus { get; set; }

    [Networked(OnChanged = nameof(onFoundPortalObj))]
    public bool toggleSearch { get; set; }
    [Networked]
    public bool networkStatusObj { get; set; }

    [Header("Sounds")]
    public AudioSource useSound;

    private void Awake()
    {
        placePortalObj = this.transform.GetChild(0).GetChild(2).gameObject;
        readyPortalObj = this.transform.GetChild(0).GetChild(3).gameObject;

        if (GameObject.FindGameObjectWithTag("Portal"))
        {
            placePortalObj.SetActive(false);
            readyPortalObj.SetActive(true);
        } 
        else
        {
            placePortalObj.SetActive(true);
            readyPortalObj.SetActive(false);
        }
    }

    public void useTeleporter(GameObject playerRef, Vector3 playerPosition)
    {
        SearchForPortal(playerRef, playerPosition);
    }

    private void SearchForPortal(GameObject playerRef, Vector3 playerPosition)
    {
        // if an open portal can be found, reference the portal, reference the player, trigger onchange.
        // if portal cannot be found, then create it.
        if (GameObject.FindGameObjectWithTag("Portal"))
        {
            portalInstanceRef = GameObject.FindGameObjectWithTag("Portal");
            portalId = portalInstanceRef.GetComponent<NetworkObject>();

            player = playerRef;

            foundPortal = true;

            networkStatusObj = false;
            toggleSearch = !toggleSearch;
            networkStatus = !networkStatus;

            return;
        }

        foundPortal = false;
        

        Runner.Spawn(portal, playerPosition, Quaternion.identity);

        networkStatusObj = true;
        toggleSearch = !toggleSearch;

        return;
    }

    static void onTogglePortal(Changed<Teleporter_USE> changed)
    {
        // if an open portal can be found, teleport the player to the portal and close the portal. 
        if (changed.Behaviour.foundPortal == false)
        {
        }
        else
        {
            // disable the player's character controller, teleport the player, then re-enable the character controller.
            // character controller overrides manual transform changes.
            CharacterController pc = changed.Behaviour.player.GetComponent<CharacterController>();
            pc.enabled = false;
            changed.Behaviour.player.transform.position = changed.Behaviour.portalInstanceRef.transform.position;
            pc.enabled = true;
            changed.Behaviour.useSound.Play();
            changed.Behaviour.Runner.Despawn(changed.Behaviour.Runner.FindObject(changed.Behaviour.portalId));
        }
    }

    static void onFoundPortalObj(Changed<Teleporter_USE> changed)
    {
        if (changed.Behaviour.networkStatusObj == true)
        {
            changed.Behaviour.placePortalObj.SetActive(false);
            changed.Behaviour.readyPortalObj.SetActive(true);
        } else
        {
            changed.Behaviour.placePortalObj.SetActive(true);
            changed.Behaviour.readyPortalObj.SetActive(false);
        }
    }

    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }
}
