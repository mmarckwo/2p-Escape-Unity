using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgTest : MonoBehaviour
{
    // on collide decrease player health.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //this.photonView.RPC("DestroyItem", RpcTarget.All);
            other.SendMessage("HealthDown", 40f);
            DestroyItem();
        }
    }

    void DestroyItem()
    {
        Destroy(gameObject);
    }
}
