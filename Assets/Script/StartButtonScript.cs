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

    // Start is called before the first frame update
    void Start()
    {
        // 假設你的PlayerController所在的遊戲物件有一個"Player"標籤
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerController = playerObject.GetComponent<PlayerController>();
        }

        // 為按鈕添加OnClick事件
        StartButton.onClick.AddListener(OnStartButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 按鈕點擊事件
    private void OnStartButtonClick()
    {
        
        if (playerController != null)
        {
            
            print("StartButtonClicked");
        }
    }
}
