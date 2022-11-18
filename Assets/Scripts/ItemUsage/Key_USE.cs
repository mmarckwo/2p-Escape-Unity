using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Key_USE : NetworkBehaviour
{
    public bool useKey()
    {
        return true;
    }

    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }
}
