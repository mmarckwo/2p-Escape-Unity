using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Teleporter_USE : NetworkBehaviour
{
    public GameObject portal;

    private GameObject portalInstanceRef;

    private NetworkId portalId { get; set; }

    private GameObject player;

    private bool foundPortal = false;

    [Networked(OnChanged = nameof(onTogglePortal))]
    public bool networkStatus { get; set; }

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

            networkStatus = !networkStatus;

            return;
        }

        foundPortal = false;

        Runner.Spawn(portal, playerPosition, Quaternion.identity);

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
            changed.Behaviour.Runner.Despawn(changed.Behaviour.Runner.FindObject(changed.Behaviour.portalId));
        }
    }

    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }
}
