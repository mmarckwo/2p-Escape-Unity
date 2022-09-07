using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{

    public string itemName = "";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // if inventory is full (true), do not collect and destroy item.
            if (other.gameObject.GetComponent<PlayerInventory>().CheckCollectItem(itemName))
            {
                Debug.Log("inventory full");
                return;
            }

            // CHANGE TO PUNRPC.
            Destroy(this.gameObject);
        }

        if (other.gameObject.tag == "Enemy")
        {
            if (itemName == "Hammer")
            {
                //other.gameObject whatever kill enemy function.
            }
        }
    }
}
