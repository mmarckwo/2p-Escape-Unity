using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MovingObject : NetworkBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private Transform targetPoint;

    public float timerInSeconds;
    public float speed;

    TickTimer movingObjTimer = TickTimer.None;
    private Vector3 velocity = Vector3.zero;

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;


        if (movingObjTimer.ExpiredOrNotRunning(Runner))
        {
            if (targetPoint == pointA)
            {
                targetPoint = pointB;
            } 
            else if (targetPoint == pointB)
            {
                targetPoint = pointA;
            } 
            else
            {
                targetPoint = pointB;
            }

            movingObjTimer = TickTimer.CreateFromSeconds(Runner, timerInSeconds);
        }

        if (movingObjTimer.IsRunning)
        {
            Vector3 startPosition = transform.position;
            transform.position = Vector3.SmoothDamp(startPosition, new Vector3(targetPoint.position.x, targetPoint.position.y, targetPoint.position.z), ref velocity, speed);
        }
    }
}
