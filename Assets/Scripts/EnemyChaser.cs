using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;

public class EnemyChaser : NetworkBehaviour
{
    private NavMeshAgent Chaser;

    private GameObject Player;

    // Start is called before the first frame update
    void Awake()
    {
        Chaser = GetComponentInParent<NavMeshAgent>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Runner.IsServer) return;

        // costly to run this per overlapping frame. is the above line is more efficient? 
        //if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        // if there is already a player being chased, continue chasing that player.
        if (Player) return;

        if (other.gameObject.tag == "Player")
        {
            Player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponent<NetworkObject>().HasStateAuthority) return;

        if (other.gameObject.tag == "Player")
        {
            Player = null;
        }
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;

        if (!Player) return;

        Vector3 directionToPlayer = transform.position - Player.transform.position;

        Vector3 newPosition = transform.position - directionToPlayer;

        Chaser.SetDestination(newPosition);
    }
}
