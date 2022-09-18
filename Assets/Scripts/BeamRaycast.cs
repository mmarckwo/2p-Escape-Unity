using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BeamRaycast : MonoBehaviour
{
    public Transform laserOrigin;
    public float maxRange = 100f;

    LineRenderer laserLine;

    private bool playerInside = false;
    private bool causeDamage = false;
    private PlayerScript hurtingPlayer;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // first point of line renderer starts at laser origin.
        laserLine.SetPosition(0, laserOrigin.localPosition);

        RaycastHit hit;

        // shoot a laser facing right. 
        if(Physics.Raycast(laserOrigin.position, laserOrigin.right, out hit, maxRange))
        {
            // if it hit something, set the x position of the line renderer's 2nd point to be at the point of collision. 
            laserLine.SetPosition(1, new Vector3((hit.point.x - this.gameObject.transform.position.x), 0, 0)); 
            if (hit.transform.tag == "Player" && playerInside == false)
            {
                // if a player is in the laser beam, get their playerscript component.
                hurtingPlayer = hit.transform.GetComponent<PlayerScript>();
                playerInside = true;
                causeDamage = true;
            } 
            else if (hit.transform.tag == "Player" && playerInside == true)
            {
                // if the player is still inside the beam, no need to get their playerscript component again. 
            }
            else
            {
                // it hits something, but it's not the player. 
                playerInside = false;
                causeDamage = false;
            }
        } 
        else
        {
            // if it doesn't hit anything, shoot the laser line to its max range.
            laserLine.SetPosition(1, new Vector3(maxRange, 0, 0));
            playerInside = false;
            causeDamage = false;
        }

        // if a player is inside, check for available damage ticks. 
        if (playerInside == true)
        {
            CheckForDmgTicks();
        }
    }

    // if there is a player inside and a damage tick is ready, start a damage tick. 
    void CheckForDmgTicks()
    {
        if (causeDamage == true) StartCoroutine(dmgTicks());
    }

    // stop causing damage for a little, deal damage, wait for seconds. if the player is still inside the laser beam after the timer, deal another tick of damage.
    IEnumerator dmgTicks()
    {
        causeDamage = false;
        hurtingPlayer.HealthDown(45f);
        yield return new WaitForSeconds(0.4f);
        if (playerInside == true) causeDamage = true;

    }
}
