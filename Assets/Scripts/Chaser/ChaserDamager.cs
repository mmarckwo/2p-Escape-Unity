using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ChaserDamager : NetworkBehaviour
{
    // similar to sawblade script.

    private bool playerInside = false;
    private bool causeDamage = false;
    private HPHandler hurtingPlayer;

    TickTimer damageTickTimer = TickTimer.None;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

            hurtingPlayer = other.GetComponent<HPHandler>();
            playerInside = true;
            causeDamage = true;
        }
    }

    // when the player leaves, stop dealing continuous damage.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        playerInside = false;
        causeDamage = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;

        // if there is no player inside, do nothing. else, cause a tick of damage.
        if (playerInside == false)
        {
            return;
        }
        else
        {
            //CheckForDmgTicks();
            if (causeDamage == true)
            {
                damageTickTimer = TickTimer.CreateFromSeconds(Runner, 0.8f);
            }
        }

        if (damageTickTimer.IsRunning)
        {
            if (causeDamage == true) hurtingPlayer.HealthDown(33f);
            causeDamage = false;
        }

        if (damageTickTimer.ExpiredOrNotRunning(Runner))
        {
            if (playerInside == true) causeDamage = true;
        }
    }
}
