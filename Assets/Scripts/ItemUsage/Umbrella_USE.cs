using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella_USE : MonoBehaviour
{
    private GameObject closedBrella;
    private GameObject openBrella;
    [HideInInspector]
    public bool status = false;

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
        if (status == false)
        {
            status = true;
            closedBrella.SetActive(false);
            openBrella.SetActive(true);
        } 
        else
        {
            status = false;
            closedBrella.SetActive(true);
            openBrella.SetActive(false);
        }
    }
}
