using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Fusion.Sockets;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPlayer _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private GameObject MainMenu;
    private String _gameName;
    private GameObject _errorMessage;

    public Transform p1SpawnPoint;
    public Transform p2SpawnPoint;

    private int _spawnCount = 0;

    CharacterInputHandler characterInputHandler;

    [SerializeField]
    private NetworkPlayer player1 { get; set; }
    [SerializeField]
    private NetworkPlayer player2 { get; set; }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        Debug.Log(player);

        if (runner.IsServer && _spawnCount == 0)
        {
            player1 = runner.Spawn(_playerPrefab, new Vector3(6.5f, 2f, -1f), Quaternion.identity, player);

            _spawnCount = 1;
        } 
        else if (runner.IsServer && _spawnCount == 1)
        {
            player2 = runner.Spawn(_playerPrefab, new Vector3(-9f, 2f, -1f), Quaternion.identity, player);
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        if (characterInputHandler == null && NetworkPlayer.Local != null)
            characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();

        if (characterInputHandler != null)
        {
            input.Set(characterInputHandler.GetNetworkInput());
        }
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) 
    {
        MainMenu.SetActive(true);
        _errorMessage.GetComponent<TextMeshProUGUI>().text = "Unable to join that room. It is likely full.";

        _runner.ProvideInput = false;
        Destroy(_runner.GetComponent<NetworkRunner>());
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) {
        Debug.Log("Scenes switched.");

        if (!runner.IsServer) return;

        try
        {
            Transform p1SpawnPoint = GameObject.FindGameObjectWithTag("P1Spawn").GetComponent<Transform>();
            player1.gameObject.GetComponent<CharacterController>().enabled = false;
            player1.gameObject.transform.position = p1SpawnPoint.transform.position;
            player1.gameObject.GetComponent<CharacterController>().enabled = true;
        } catch (Exception)
        {}

        try
        {
            Transform p2SpawnPoint = GameObject.FindGameObjectWithTag("P2Spawn").GetComponent<Transform>();
            player2.gameObject.GetComponent<CharacterController>().enabled = false;
            player2.gameObject.transform.position = p2SpawnPoint.transform.position;
            player2.gameObject.GetComponent<CharacterController>().enabled = true;

        }
        catch (Exception)
        {}
    }
    public void OnSceneLoadStart(NetworkRunner runner) {
        Debug.Log("Switching scenes...");
    }

    private NetworkRunner _runner;

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = _gameName,
            Scene = SceneManager.GetActiveScene().buildIndex + 1,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI()
    {
        //if (_runner == null)
        //{
        //    if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
        //    {
        //        StartGame(GameMode.Host);
        //    }
        //    if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
        //    {
        //        StartGame(GameMode.Client);
        //    }
        //}
    }

    private void Start()
    {
        MainMenu = transform.GetChild(0).gameObject;
        _errorMessage = transform.GetChild(0).GetChild(3).gameObject;
    }

    public void StartGameButton()
    {
        if (_runner == null)
        {
            _gameName = transform.GetChild(0).GetChild(2).gameObject.GetComponent<TMP_InputField>().text;

            if (_gameName == "")
            {
                _errorMessage.SetActive(true);
                _errorMessage.GetComponent<TextMeshProUGUI>().text = "Please enter a room name.";
                return;
            }

            StartGame(GameMode.AutoHostOrClient);
            MainMenu.SetActive(false);
        }
    }
}
