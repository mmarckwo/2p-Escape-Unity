using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Flashlight_USE : NetworkBehaviour
{
    private Light spotlight;
    [Networked(OnChanged = nameof(onToggleChange))]
    public bool networkStatus { get; set; }

    public Material LightOn;
    public Material LightOff;

    private Material lightArea;

    private void Awake()
    {
        spotlight = this.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Light>();
        lightArea = this.transform.GetChild(0).GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[1];
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
            // light is off.
            changed.Behaviour.spotlight.enabled = false;
            changed.Behaviour.lightArea = changed.Behaviour.LightOff;
        } else
        {
            // light is on.
            changed.Behaviour.spotlight.enabled = true;
            changed.Behaviour.lightArea = changed.Behaviour.LightOn;
        }
    }

    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }

}
