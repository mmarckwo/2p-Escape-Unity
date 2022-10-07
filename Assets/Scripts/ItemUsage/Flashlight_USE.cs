using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Flashlight_USE : NetworkBehaviour
{
    private Light spotlight;
    [Networked(OnChanged = nameof(onToggleChange))]
    public bool networkStatus { get; set; }

    private void Awake()
    {
        spotlight = this.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Light>();
    }

    public void toggleLight()
    {
        if (networkStatus == false)
        {
            networkStatus = true;
        }
        else
        {
            networkStatus = false;
        }
    }

    static void onToggleChange(Changed<Flashlight_USE> changed)
    {
        if (changed.Behaviour.networkStatus == false)
        {
            changed.Behaviour.spotlight.enabled = false;
        } else
        {
            changed.Behaviour.spotlight.enabled = true;
        }
    }

    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }

}
