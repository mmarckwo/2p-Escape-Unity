using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion;

public class PlayerScript : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    private CharacterController _originalController;
    private NetworkCharacterControllerPrototype _networkController;
    public float originalStepOffset;

    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    Camera localCamera;

    private NetworkObject playerNetworkObj;
    public GameObject playerGameUI;
    public GameObject pauseMenu;
    public GameObject tipMenu;
    public GameObject notifyScreen;

    private Button restartButton;

    [Header("Physics")]
    public float jumpForce = 22.5f;
    public float speed = 10.0f;
    [HideInInspector]
    Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f); // How fast we are going.
    // player will use its own gravity for jump physics.
    public float gravityScale = 2.5f;
    public static float globalGravity = -9.81f;
    [HideInInspector]
    public int umbrellaFloat = 1;

    [Networked]
    public bool controlsEnabled { get; set; }
    [Networked]
    public bool enablePausing { get; set; }

    private int enableControllerCounter = 0;

    public string currentScene;

    void Awake()
    { 
        _originalController = GetComponent<CharacterController>();
        //_networkController = GetComponent<NetworkCharacterControllerPrototype>();

        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        playerNetworkObj = GetComponent<NetworkObject>();

        localCamera = GetComponentInChildren<Camera>();

        originalStepOffset = _originalController.stepOffset;
    }

    public override void FixedUpdateNetwork()
    {
        // get input from the network
        if (GetInput(out NetworkInputData networkInputData))
        {
            // rotate the transform according to the client aim vector.
            transform.forward = networkInputData.aimForwardVector;

            // cancel out x axis rotation to prevent tilting on character model.
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            // move.
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;

            if (!controlsEnabled)
            {
                moveDirection = new Vector3(0, 0, 0);
            };

            moveDirection.Normalize();

            networkCharacterControllerPrototypeCustom.Move(moveDirection);

            if (networkInputData.isEscButtonPressed)
            {
                // free the cursor regardless if you can pause or not.
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // if pausing isn't enabled, don't pause.
                if (!enablePausing) return;

                // if the game isn't paused, pause the game.
                if (controlsEnabled == true)
                {
                    controlsEnabled = false;

                    // if we're not pausing locally, then we don't wanna see the other player's pause menu.
                    if (!playerNetworkObj.HasInputAuthority) return;

                    pauseMenu.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                // if the game is already paused, continue the game.
                else
                {
                    controlsEnabled = true;

                    // if we're not pausing locally, then we don't wanna see the other player's pause menu.
                    if (!playerNetworkObj.HasInputAuthority) return;

                    pauseMenu.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = true;
                }
            }

            if (!controlsEnabled) return;

            // jump
            if (networkInputData.isJumpPressed)
            {
                networkCharacterControllerPrototypeCustom.Jump();
            }
                
        }
    }

    private void Update()
    {
        // handles local cursor lock state based on if player is paused or not.
        if (!playerNetworkObj.HasInputAuthority) return;

        // get button from pause menu if not state authority set button to disabled?

        if (!controlsEnabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void UmbrellaFall()
    {
        networkCharacterControllerPrototypeCustom.UmbrellaFall();
    }

    void Jump()
    {

        // if the umbrella is open, greatly reduce jump height. 
        if (umbrellaFloat != 1)
        {
            velocity.y = Mathf.Sqrt((jumpForce / 4f) * -2f * globalGravity / umbrellaFloat);
        }
        else
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * globalGravity);
        }
        
    }

    public void PlayerJoined(PlayerRef player)
    {
        // to enable player 1 movement. if another player joins and they don't have input authority, then that means it's the second player.
        if (player != Object.InputAuthority)
        {
            notifyScreen.SetActive(false);

            enableControllerCounter++;
            enablerCounter();
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        controlsEnabled = false;
        enablePausing = false;

        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("PlayerLeftScene");
    }

    // for client to remove wait for player screen.
    public override void Spawned()
    {
        controlsEnabled = false;
        enablePausing = false;

        // get the restart button. disable it for the client. 
        restartButton = pauseMenu.transform.GetChild(3).GetComponent<Button>();
        if (!Object.HasStateAuthority) restartButton.interactable = false;

        if (!Runner.IsServer)
        {
            tipMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        };

        var enumerator = Runner.ActivePlayers.GetEnumerator();
        int count = 0;

        while (enumerator.MoveNext())
        {
            count++;
        }

        // if there are two player connect to this session, then close the "wait for player" screen.
        if (count == 2) WaitForPlayerClose();
    }

    public void WaitForPlayerClose()
    {
        notifyScreen.SetActive(false);

        //if (!playerNetworkObj.HasStateAuthority) return;

        enableControllerCounter++;
        enablerCounter();
    }

    public void PauseMenuClose()
    {
        // close pause menu and continue game.
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        RPC_ResumeControlFromPause();
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_ResumeControlFromPause()
    {
        controlsEnabled = true;
    }

    public void RestartRoom()
    {
        currentScene = SceneManager.GetActiveScene().name;

        Runner.SetActiveScene("ReloadScene");

        PauseMenuClose();
    }

    public void TipMenuClose()
    {
        tipMenu.SetActive(false);

        if (!playerNetworkObj.HasStateAuthority) return;

        enableControllerCounter++;
        enablerCounter();

    }

    private void enablerCounter()
    {
        if (!playerNetworkObj.HasInputAuthority)
        {
            if (enableControllerCounter == 1)
            {
                enablePausing = true;
                controlsEnabled = true;
            }
        }
        // player with input authority but no state authority is the client. enable playerUI on their end.
        else if (playerNetworkObj.HasInputAuthority && !playerNetworkObj.HasStateAuthority)
        {
            if (enableControllerCounter == 1)
            {
                playerGameUI.SetActive(true);

                Cursor.lockState = CursorLockMode.Locked;
            }
        };

        if (!playerNetworkObj.HasStateAuthority) return;

        // when the tip menu has been cleared and a player has joined, enable controls. 
        if (enableControllerCounter == 2)
        {
            playerGameUI.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;

            enablePausing = true;
            controlsEnabled = true;
        }
    }
}
