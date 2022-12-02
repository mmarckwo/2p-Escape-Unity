using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemInteraction : NetworkBehaviour
{

    public string itemName = "";

    [Networked(OnChanged = nameof(OnHitSound))]
    private bool onHitSoundPlay { get; set; }
    [Header("Sounds")]
    public AudioSource hitSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // only the host can watch players collect items.
            if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

            // if inventory is full (true), do not collect and destroy item.
            if (other.gameObject.GetComponent<PlayerInventory>().CheckCollectItem(itemName))
            {
                return;
            }
            else
            {
                Runner.Despawn(Object);
            }
            
        }

        if (other.gameObject.tag == "Enemy")
        {
            if (itemName == "Hammer")
            {
                onHitSoundPlay = !onHitSoundPlay;
                Runner.Despawn(other.gameObject.GetComponent<NetworkObject>());
            }
        }
    }

    static void OnHitSound(Changed<ItemInteraction> changed)
    {
        changed.Behaviour.hitSound.Play();
    }
}
