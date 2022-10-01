using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Umbrella_USE : NetworkBehaviour
{
    private GameObject closedBrella;
    private GameObject openBrella;
    [HideInInspector]
    public bool status = false;
    [Networked(OnChanged = nameof(onCanopyChange))]
    public bool networkStatus { get; set; }

    private void Awake()
    {
        closedBrella = this.transform.GetChild(0).GetChild(0).gameObject;
        openBrella = this.transform.GetChild(0).GetChild(1).gameObject;

        openBrella.SetActive(false);
    }

    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }

    public void toggleCanopy()
    {
        if (networkStatus == false)
        {
            networkStatus = true;
            //closedBrella.SetActive(false);
            //openBrella.SetActive(true);
        }
        else
        {
            networkStatus = false;
            //closedBrella.SetActive(true);
            //openBrella.SetActive(false);
        }
    }

    static void onCanopyChange(Changed<Umbrella_USE> changed)
    {
        if (changed.Behaviour.networkStatus == false)
        {
            changed.Behaviour.closedBrella.SetActive(true);
            changed.Behaviour.openBrella.SetActive(false);
        } else
        {
            changed.Behaviour.closedBrella.SetActive(false);
            changed.Behaviour.openBrella.SetActive(true);
        }
    }
}
