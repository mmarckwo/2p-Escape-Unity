using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter_USE : MonoBehaviour
{
    public GameObject portal;
    private bool openPortal = false;
    static bool portalInstantiated = false;
    private GameObject portalInstance;

    public void useTeleporter(GameObject player)
    {
        // if the portal is not open, create one, say the portal is open. if portal is open, teleport the player to the portal and close the portal. 
        if (!openPortal)
        {
            // if the portal hasn't been created yet, create it, reference created portal. else, set it back to active.
            if(!portalInstantiated)
            {
                portalInstance = Instantiate(portal, transform.position, Quaternion.identity);
                portalInstantiated = true;
            } 
            else
            {
                portalInstance.transform.position = player.transform.position;
                portalInstance.SetActive(true);
            }

            openPortal = true;
        } 
        else
        {
            // disable the player's character controller, teleport the player, then re-enable the character controller.
            // character controller overrides manual transform changes.
            CharacterController pc = player.GetComponent<CharacterController>();
            pc.enabled = false;
            player.transform.position = portalInstance.transform.position;
            pc.enabled = true;
            portalInstance.SetActive(false);
            openPortal = false;
        }

    }
}
