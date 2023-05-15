﻿using System.Collections;
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
    private int playerNumber;
    public PlayerController PlayerController;
    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();
    [SerializeField]
    public GameMode gameMode;
    public int levelIndex; // 使用int类型的levelIndex来表示当前选中的关卡
    private Dictionary<int, Dictionary<int, Vector3>> spawnPositions;
    private void Start()
    {
        spawnPositions = new Dictionary<int, Dictionary<int, Vector3>>()
        {
            { 1, new Dictionary<int, Vector3>()
                {
                    { 1, new Vector3(0, 2, 0) },
                    { 2, new Vector3(0, 2, 2F) }
                }
            },
            // 添加更多关卡和生成位置
        };

        //StartGame(gameMode);
    }

    public async void StartGame(GameMode mode,int selectedLevel,int PlayerNum)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not set.");
            return;
        }
        levelIndex = selectedLevel;
        playerNumber=PlayerNum;
        networkRunner.ProvideInput = true;
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 spawnPosition;
        if (spawnPositions.TryGetValue(levelIndex, out Dictionary<int, Vector3> levelSpawnPositions))
        {
            if (levelSpawnPositions.TryGetValue(playerNumber, out Vector3 playerSpawnPosition))
            {
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
            Debug.LogError("Spawn position not found for the selected level index.");
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
        data.buttons.Set(InputButtons.JUMP,Input.GetKey(KeyCode.Space));
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