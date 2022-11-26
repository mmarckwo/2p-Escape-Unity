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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Player = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player) return;

        Vector3 directionToPlayer = transform.position - Player.transform.position;

        Vector3 newPosition = transform.position - directionToPlayer;

        Chaser.SetDestination(newPosition);
    }
}
