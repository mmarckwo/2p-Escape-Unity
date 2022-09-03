using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPickup : MonoBehaviour
{
    // on collide increase player health.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //this.photonView.RPC("DestroyItem", RpcTarget.All);
            other.SendMessage("HealthUp", 20f);
            DestroyItem();
        }
    }

    void DestroyItem()
    {
        Destroy(gameObject);
    }
}
