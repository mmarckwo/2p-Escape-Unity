using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeCollide : MonoBehaviour
{

    private bool playerInside = false;
    private bool causeDamage = false;
    private PlayerScript hurtingPlayer;

    // on collide, get player script from colliding player. say there is that player within the sawblade, and it needs to deal damage.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hurtingPlayer = other.GetComponent<PlayerScript>();
            playerInside = true;
            causeDamage = true;

        }
    }

    // when the player leaves, stop dealing continuous damage.
    private void OnTriggerExit(Collider other)
    {
        playerInside = false;
        causeDamage = false;
    }
    
    void Update()
    {
        // if there is no player inside, do nothing. else, cause a tick of damage.
        if (playerInside == false)
        {
            return;
        } 
        else
        {
            CheckForDmgTicks();
        }
    }

    // if there is a player inside and a damage tick is ready, start a damage tick. 
    void CheckForDmgTicks()
    {
        if (causeDamage == true) StartCoroutine(dmgTicks());
    }

    // stop causing damage for a little, deal damage, wait for seconds. if the player is still inside the sawblade after the timer, deal another tick of damage.
    IEnumerator dmgTicks()
    {
        causeDamage = false;
        hurtingPlayer.HealthDown(40f);
        yield return new WaitForSeconds(0.8f);
        if (playerInside == true) causeDamage = true;

    }
}
