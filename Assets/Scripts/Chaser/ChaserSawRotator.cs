using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ChaserSawRotator : NetworkBehaviour
{
    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;

        transform.Rotate(new Vector3(0, 0, 120) * Runner.DeltaTime);
    }
}
