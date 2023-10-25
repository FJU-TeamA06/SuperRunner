using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEngine.InputSystem;
public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] 
    private float _mouseSensitivity = 10f;
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
    public bool SideInput=true;
    public bool EnableInput=true;
    public InputActionAsset myActions;
    public bool PersonalTestMode=false;
    private string MySessionName="Fusion Room";
    private void Awake() {
        if(PersonalTestMode)
        {
            MySessionName="Fusion Room1";
        }
    }
    private void Start()
    {
        EnableInputToggle(true);
        SideInputToggle(true);
        myActions.Enable();
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
                }
            },
            { 3, new Dictionary<int, Vector3>()
                {
                    { 1, new Vector3(0, 62, -200) },
                    { 2, new Vector3(0, 62, -202F) }
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
            SessionName = MySessionName,
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
            SessionName = MySessionName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    public void SideInputToggle(bool _sideinput)
    {
        SideInput = _sideinput;
    }
    public void EnableInputToggle(bool _enableinput)
    {
        EnableInput = _enableinput;
    }
    public bool GetEnableInput()
    {
        return EnableInput;
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
        InputAction Up = myActions.FindAction("Up");
        float UpValue = Up.ReadValue<float>();
        InputAction Down = myActions.FindAction("Down");
        float DownValue = Down.ReadValue<float>();
        InputAction Left = myActions.FindAction("Left");
        float LeftValue = Left.ReadValue<float>();
        InputAction Right = myActions.FindAction("Right");
        float RightValue = Right.ReadValue<float>();
        if (SceneManager.GetActiveScene().name == "FPS")
        {
            if (Input.GetKey(KeyCode.W)) { data.MoveInput += Vector2.up; }
            if (Input.GetKey(KeyCode.S)) { data.MoveInput += Vector2.down; }
            if (Input.GetKey(KeyCode.D)) { data.MoveInput += Vector2.left; }
            if (Input.GetKey(KeyCode.A)) { data.MoveInput += Vector2.right; }

        }
        else
        {
            if(EnableInput)
            {
                if(SideInput == true)
                {
                    if (Input.GetKey(KeyCode.D)) { data.MoveInput += Vector2.up; }
                    if (RightValue>0){ data.MoveInput += Vector2.up; }
                    if (Input.GetKey(KeyCode.A)) { data.MoveInput += Vector2.down; }
                    if (LeftValue>0){ data.MoveInput += Vector2.down; }
                    if (Input.GetKey(KeyCode.S)) { data.MoveInput += Vector2.left; }
                    if (DownValue>0) { data.MoveInput += Vector2.left; }
                    if (Input.GetKey(KeyCode.W)) { data.MoveInput += Vector2.right; }
                    if (UpValue>0) { data.MoveInput += Vector2.right; }
                }
                else
                {
                    if (Input.GetKey(KeyCode.W)) { data.MoveInput += Vector2.up; }
                    if (UpValue>0){data.MoveInput += new Vector2(0,UpValue);}
                    if (Input.GetKey(KeyCode.S)) { data.MoveInput += Vector2.down; }
                    if (DownValue>0){data.MoveInput += new Vector2(0,DownValue*-1);}
                    if (Input.GetKey(KeyCode.D)) { data.MoveInput += Vector2.left; }
                    if (LeftValue>0){data.MoveInput += new Vector2(LeftValue,0);}
                    if (Input.GetKey(KeyCode.A)) { data.MoveInput += Vector2.right; }
                    if (RightValue>0){data.MoveInput += new Vector2(RightValue*-1,0);}

                    InputAction RightPitch = myActions.FindAction("RightPitch");
                    //print(RightPitch.ReadValue<float>());
                    InputAction RightYaw = myActions.FindAction("RightYaw");
                    data.Pitch = (Input.GetAxis("Mouse Y") * _mouseSensitivity)+RightPitch.ReadValue<float>();
                    data.Yaw = Input.GetAxis("Mouse X") * _mouseSensitivity+RightYaw.ReadValue<float>() ;
                }
            }
            
            
            
        }
        
        InputAction JUMP = myActions.FindAction("A");
        InputAction Fire = myActions.FindAction("B");
        data.buttons.Set(InputButtons.JUMP, Input.GetKey(KeyCode.Space)||JUMP.ReadValue<float>() > 0.5f);
        data.buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse0)||Fire.ReadValue<float>() > 0.5f);
        input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // 在這裡添加處理缺失輸入的邏輯
        // 例如提供一個預設的輸入
        var defaultInput = new NetworkInputData();
        input.Set(defaultInput);

        // 或者記錄一個錯誤信息
        Debug.LogError($"Input missing for player ");

    }
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