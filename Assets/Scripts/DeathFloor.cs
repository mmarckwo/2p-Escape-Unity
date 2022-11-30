using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DeathFloor : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

            other.GetComponent<HPHandler>().HealthDown(200f);

        }
    }
}
