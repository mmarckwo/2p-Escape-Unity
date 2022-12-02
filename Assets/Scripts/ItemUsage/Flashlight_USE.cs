using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Flashlight_USE : NetworkBehaviour
{
    private Light spotlight;
    [Networked(OnChanged = nameof(onToggleChange))]
    public bool networkStatus { get; set; }

    private GameObject OnFlashlight;
    private GameObject OffFlashlight;

    [Header("Sounds")]
    public AudioSource flashlightSound;

    private void Awake()
    {
        spotlight = this.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Light>();

        OnFlashlight = this.transform.GetChild(0).GetChild(2).gameObject;
        OffFlashlight = this.transform.GetChild(0).GetChild(3).gameObject;

        OffFlashlight.SetActive(false);

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

            changed.Behaviour.OnFlashlight.SetActive(false);
            changed.Behaviour.OffFlashlight.SetActive(true);
        } else
        {
            // light is on.
            changed.Behaviour.spotlight.enabled = true;

            changed.Behaviour.OnFlashlight.SetActive(true);
            changed.Behaviour.OffFlashlight.SetActive(false);
        }
        changed.Behaviour.flashlightSound.Play();
    }

    public void Init(float throwSpeed)
    {
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
    }

}
