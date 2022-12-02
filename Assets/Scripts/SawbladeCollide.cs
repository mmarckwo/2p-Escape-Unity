using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SawbladeCollide : NetworkBehaviour
{

    private bool playerInside = false;
    private bool causeDamage = false;
    private HPHandler hurtingPlayer;

    TickTimer damageTickTimer = TickTimer.None;

    public float turnSpeed = 120f;

    [Header("Sounds")]
    public AudioSource hurtSound;
    [Networked(OnChanged = nameof(OnHurtSoundPlay))]
    private bool hurtSoundPlay { get; set; }

    // on collide, get player script from colliding player. say there is that player within the sawblade, and it needs to deal damage.
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

        transform.Rotate(new Vector3(0, turnSpeed, 0) * Runner.DeltaTime);

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
            if (causeDamage == true) hurtingPlayer.HealthDown(40f);
            hurtSoundPlay = !hurtSoundPlay;
            causeDamage = false;
        }

        if (damageTickTimer.ExpiredOrNotRunning(Runner))
        {
            if (playerInside == true) causeDamage = true;
        }
    }

    static void OnHurtSoundPlay(Changed<SawbladeCollide> changed)
    {
        changed.Behaviour.hurtSound.Play();
    }

    // if there is a player inside and a damage tick is ready, start a damage tick. 
    //void CheckForDmgTicks()
    //{
    //    if (causeDamage == true) StartCoroutine(dmgTicks());
    //}

    // stop causing damage for a little, deal damage, wait for seconds. if the player is still inside the sawblade after the timer, deal another tick of damage.
    //IEnumerator dmgTicks()
    //{
    //    causeDamage = false;
    //    hurtingPlayer.HealthDown(40f);
    //    yield return new WaitForSeconds(0.8f);
    //    if (playerInside == true) causeDamage = true;
    //}
}
