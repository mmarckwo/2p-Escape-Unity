using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Umbrella_USE : NetworkBehaviour
{
    private GameObject closedBrella;
    private GameObject openBrella;
    [Networked(OnChanged = nameof(onCanopyChange))]
    public bool networkStatus { get; set; }

    TickTimer canopyToggleTimer = TickTimer.None;
    private bool canToggle = true;

    [Header("Sounds")]
    public AudioSource openSound;
    public AudioSource closeSound;

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
        if (canToggle)
        {
            if (networkStatus == false)
            {
                networkStatus = true;
            }
            else
            {
                networkStatus = false;
            }

            canToggle = false;
            // set a cooldown for toggling canopy to prevent the player from gliding extra long distances by rapidly toggling.
            canopyToggleTimer = TickTimer.CreateFromSeconds(Runner, 0.5f);
        }
    }

    static void onCanopyChange(Changed<Umbrella_USE> changed)
    {
        if (changed.Behaviour.networkStatus == false)
        {
            // close.
            changed.Behaviour.closedBrella.SetActive(true);
            changed.Behaviour.openBrella.SetActive(false);
            changed.Behaviour.closeSound.Play();
        } else
        {
            // open.
            changed.Behaviour.closedBrella.SetActive(false);
            changed.Behaviour.openBrella.SetActive(true);
            changed.Behaviour.openSound.Play();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // re-enable toggling when timer expires.
        if (canopyToggleTimer.ExpiredOrNotRunning(Runner))
        {
            canToggle = true;

            // prevent timer from being triggered again.
            canopyToggleTimer = TickTimer.None;
        }
    }
}
