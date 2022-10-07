using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Teleporter_USE : NetworkBehaviour
{
    public GameObject portal;

    private GameObject portalInstanceRef;

    private NetworkId portalId;

    // SET NETWORKED VALUE MAKE USE TELEPORTER CHANGE VALUE DO THE THINGS IN ONCHANGE FUNCTION.

    public void useTeleporter(GameObject player)
    {
        // if an open portal can't be found, create one. if an open portal can be found, teleport the player to the portal and close the portal. 
        if (!SearchForPortal())
        {
            Runner.Spawn(portal, transform.position, Quaternion.identity);
        } 
        else
        {
            // disable the player's character controller, teleport the player, then re-enable the character controller.
            // character controller overrides manual transform changes.
            CharacterController pc = player.GetComponent<CharacterController>();
            pc.enabled = false;
            player.transform.position = portalInstanceRef.transform.position;
            pc.enabled = true;
            Runner.Despawn(Runner.FindObject(portalId));
        }

    }

    private bool SearchForPortal()
    {
        if (GameObject.FindGameObjectWithTag("Portal"))
        {
            portalInstanceRef = GameObject.FindGameObjectWithTag("Portal");
            portalId = portalInstanceRef.GetComponent<NetworkObject>();

            return true;
        }

        return false;
    }

    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }
}
