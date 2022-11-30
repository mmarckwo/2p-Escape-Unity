using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class RestartButton : NetworkBehaviour
{
    private BasicSpawner spawner;
    private PlayerScript p1Script;
    private PlayerScript p2Script;

    private bool p1ControlStatus;
    private bool p2ControlStatus;

    public Button button; 

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            button.interactable = false;
            return;
        }

        try
        {
            spawner = GameObject.FindGameObjectWithTag("BasicSpawner").GetComponent<BasicSpawner>();
            spawner.player1.GetComponent<HPHandler>().health = 100f;
            spawner.player2.GetComponent<HPHandler>().health = 100f;

            p1Script = spawner.player1.GetComponent<PlayerScript>();
            p2Script = spawner.player2.GetComponent<PlayerScript>();

            p1ControlStatus = p1Script.controlsEnabled;
            p2ControlStatus = p2Script.controlsEnabled;

            p1Script.controlsEnabled = false;
            p2Script.controlsEnabled = false;

            p1Script.enablePausing = false;
            p2Script.enablePausing = false;
        }
        catch (Exception)
        {
            Debug.Log("error");
        }
    }

    public void Retry()
    {
        if (!Object.HasStateAuthority) return;

        p1Script.controlsEnabled = p1ControlStatus;
        p2Script.controlsEnabled = p2ControlStatus;

        p1Script.enablePausing = true;
        p2Script.enablePausing = true;

        Runner.SetActiveScene("ReloadScene");
    }
}
