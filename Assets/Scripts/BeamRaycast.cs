using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[ExecuteInEditMode]
public class BeamRaycast : NetworkBehaviour
{
    public Transform laserOrigin;
    public float maxRange = 100f;

    LineRenderer laserLine;

    private bool playerInside = false;
    private bool causeDamage = false;
    private HPHandler hurtingPlayer;

    public LayerMask collisionLayers;

    TickTimer dmgTickTimer = TickTimer.None;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    public override void FixedUpdateNetwork()
    {
        // first point of line renderer starts at laser origin.
        laserLine.SetPosition(0, laserOrigin.localPosition);

        Runner.LagCompensation.Raycast(laserOrigin.position, laserOrigin.right, maxRange, Object.InputAuthority, out var hit, collisionLayers, HitOptions.IncludePhysX);
        float hitDistance = maxRange;
        playerInside = false;

        if (hit.Distance > 0)
            hitDistance = hit.Distance;

        laserLine.SetPosition(1, new Vector3(hitDistance, 0, 0));

        // hit player.
        if (hit.Hitbox != null && playerInside == false)
        {

            playerInside = true;
            causeDamage = true;


            if (Object.HasStateAuthority)
                hurtingPlayer = hit.Hitbox.transform.root.GetComponent<HPHandler>();
        }
        else if (hit.Hitbox != null && playerInside == true)
        {
            // player still inside beam, do not set values again.
        }
        // hit not player.
        else if (hit.Collider != null)
        {
            playerInside = false;
            causeDamage = false;
        } 
        else
        {
            // just in case.
            playerInside = false;
            causeDamage = false;
        }

        // if a player is inside, check for available damage ticks. 
        if (playerInside == true)
        {
            if (!dmgTickTimer.ExpiredOrNotRunning(Runner))
            {
                causeDamage = false;
            }

            if (causeDamage == true)
            {
                dmgTickTimer = TickTimer.CreateFromSeconds(Runner, .4f);

                //causeDamage = false;
                if (Object.HasStateAuthority)
                    hurtingPlayer.HealthDown(45f);

            }
        }
    }
}
