using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemInteraction : NetworkBehaviour
{

    public string itemName = "";
    bool isCollected;
    bool isCollecting = false;

    private void OnTriggerExit(Collider other)
    {
        isCollecting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // if inventory is full (true), do not collect and destroy item.
            if (isCollecting == false)
            {
                isCollecting = true;

                if (other.gameObject.GetComponent<PlayerInventory>().CheckCollectItem(itemName))
                {
                    return;
                } else
                {
                    isCollected = true;
                }
            }

            //Destroy(this.gameObject);
            //isCollected = true;
        }

        if (other.gameObject.tag == "Enemy")
        {
            if (itemName == "Hammer")
            {
                //other.gameObject whatever kill enemy function.
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isCollected == true)
        {
            Runner.Despawn(Object);
        }
    }
}
