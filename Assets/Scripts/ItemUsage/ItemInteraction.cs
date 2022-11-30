using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemInteraction : NetworkBehaviour
{

    public string itemName = "";

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
                Runner.Despawn(other.gameObject.GetComponent<NetworkObject>());
            }
        }
    }
}
