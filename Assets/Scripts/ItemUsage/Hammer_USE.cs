using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Hammer_USE : NetworkBehaviour
{
    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }
}
