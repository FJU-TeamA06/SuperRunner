using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner networkRunner = null;

    [SerializeField]
    private NetworkPrefabRef playerPrefab;
    
    public PlayerController PlayerController;
    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();
    [SerializeField]
    public GameMode gameMode;
    public int playerNumber; //
    public int levelIndex; // 使用int类型的levelIndex来表示当前选中的关卡
    private Dictionary<int, Dictionary<int, Vector3>> spawnPositions;
    private bool isServer = false;

    private void Start()
    {
        //PlayerController playerController = GetComponent<PlayerController>();
        //playerController.SetBasicSpawner(this);   //傳遞

        spawnPositions = new Dictionary<int, Dictionary<int, Vector3>>()
        {
            { 1, new Dictionary<int, Vector3>()
                {
                    { 1, new Vector3(0, 2, 0) },
                    { 2, new Vector3(0, 2, 2F) }
                }
            },
            { 2, new Dictionary<int, Vector3>()
                {
                    { 1, new Vector3(0, 2, 200) },
                    { 2, new Vector3(0, 2, 202F) }
                    // 在這裡添加第二個關卡的初始位置設定
                }
            },
        };
        if (IsServerMode())
        {
            StartServer(gameMode);
        }
        //StartGame(gameMode);
    }

  


    public async void StartServer(GameMode mode)
    {
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    public async void StartGame(GameMode mode, int selectedLevel, int PlayerNum)
    {//初始化房間和自動配對
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not set.");
            return;
        }
        levelIndex = selectedLevel;
        playerNumber = PlayerNum;
        networkRunner.ProvideInput = true;  //tell runner we provide input
        //print("OK");
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

   

    public Vector3 GetSpawnPosition(int levelIndex, int playerNumber)
    {
        if (spawnPositions.TryGetValue(levelIndex, out Dictionary<int, Vector3> levelSpawnPositions))
        {
            if (levelSpawnPositions.TryGetValue(playerNumber, out Vector3 playerSpawnPosition))
            {
                return playerSpawnPosition;
            }
        }
        // 如果找不到則返回默認重生位置
        return Vector3.zero;
    }

    private bool IsServerMode()
    {
        // 判斷是否為 Server 模式的程式碼...
        if (gameMode == GameMode.Server)
        {
            isServer = true;
            return true;
        }
        else
        {
            return false;
        }

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 spawnPosition;
        if (spawnPositions.TryGetValue(levelIndex, out Dictionary<int, Vector3> levelSpawnPositions))
        {
            if (levelSpawnPositions.TryGetValue(playerNumber, out Vector3 playerSpawnPosition))
            {
                //Debug.Log(playerNumber);//P1 or P2
                //Debug.Log(playerSpawnPosition);//P1 or P2
                NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, playerSpawnPosition, Quaternion.identity, player);
                playerList.Add(player, networkPlayerObject);
            }
            else
            {
                // 处理无法找到 PlayerNum 的逻辑
            }
        }
        else
        {
            if (isServer)
            { 
                NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity, player);
                playerList.Add(player, networkPlayerObject);
            }
            else
            {
                Debug.LogError("Spawn position not found for the selected level index.");
            }

        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerList.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.A))
            data.movementInput += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            data.movementInput += Vector3.right;
        if (Input.GetKey(KeyCode.W))
            data.movementInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            data.movementInput += Vector3.back;
        if (Input.GetKey(KeyCode.UpArrow))
            data.movementInput += Vector3.right;
        if (Input.GetKey(KeyCode.DownArrow))
            data.movementInput += Vector3.left;
        if (Input.GetKey(KeyCode.LeftArrow))
            data.movementInput += Vector3.forward;
        if (Input.GetKey(KeyCode.RightArrow))
            data.movementInput += Vector3.back;
        data.buttons.Set(InputButtons.JUMP, Input.GetKey(KeyCode.Space));
        data.buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse0));
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
}