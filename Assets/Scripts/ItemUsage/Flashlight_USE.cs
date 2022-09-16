using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight_USE : MonoBehaviour
{
    private Light spotlight;

    private void Awake()
    {
        spotlight = this.transform.GetChild(1).gameObject.GetComponent<Light>();
    }

    public void toggleLight()
    {
        spotlight.enabled = !spotlight.enabled;
    }

}
