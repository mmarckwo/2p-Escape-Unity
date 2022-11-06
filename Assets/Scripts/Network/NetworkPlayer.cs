using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft, IPlayerJoined
{
    public static NetworkPlayer Local { get; set; }

    public Transform playerModel;
    public GameObject player1Model;
    public GameObject player2Model;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            Debug.Log("Spawned local player");
        }
        else
        {
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            Canvas canvas = GetComponentInChildren<Canvas>();
            canvas.enabled = false;

            Debug.Log("Spawned remote player");
        }

        // make it easier to tell which player is which.
        transform.name = $"Player_{Object.Id}";
        DontDestroyOnLoad(this);
    }

    public void PlayerJoined(PlayerRef player)
    {
        // first spawned player starts as player 2 model, but switches to player 1 model when player 2 joins.
        if (player != Object.InputAuthority)
        {
            // set to player 1 model.
            player1Model.SetActive(true);
            player2Model.SetActive(false);
        } else
        {
            // set to player 2 model.
            player1Model.SetActive(false);
            player2Model.SetActive(true);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }


}
