using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class StartButtonScript : NetworkBehaviour
{
    [SerializeField]
    public Button StartButton;
    [SerializeField]
    private PlayerController playerController;
    public GameObject wallObject;

    public override void Spawned()
    {
        // 假設你的PlayerController所在的遊戲物件有一個"Player"標籤
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerController = playerObject.GetComponent<PlayerController>();
        }

        // 移除所有旧的事件监听器
        StartButton.onClick.RemoveAllListeners();
        // 為按鈕添加OnClick事件
        StartButton.onClick.AddListener(OnStartButtonClick);
        wallObject = GameObject.FindGameObjectWithTag("StartWall");
        if (wallObject == null)
        {
            Debug.LogWarning("未找到帶有 'Wall' 標籤的物件。請確保牆物件已設置標籤。");
        }
    }   
    

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        
    }

    // 按鈕點擊事件
    private void OnStartButtonClick()
    {
        if (playerController != null)
            {
                if (wallObject != null)
                {
                    StartWall startWallScript = wallObject.GetComponent<StartWall>();
                    if (startWallScript != null)
                    {
                        startWallScript.RequestDespawnWall_RPC();
                        DisableStartButton_RPC();
                        
                    }
                    else
                    {
                        Debug.LogWarning("StartWall 腳本未找到，請確保已將其添加到 wallObject 物件上。");
                    }
                }
            }
        
    }
    //集體禁用開始按鈕的函式，之後可以在這邊開始遊戲(eg.計時器、BGM等等)
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void DisableStartButton_RPC()
    {
        StartButton.gameObject.SetActive(false);
        print("Disabled Start Button");
    }
    
}
