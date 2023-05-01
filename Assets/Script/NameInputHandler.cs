using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;

public class NameInputHandler : MonoBehaviour
{
    public InputField nameInputField;
    public Button okButton;
    private string playerName = "";
    public BasicSpawner basicSpawner;
    public GameMode gameMode;

    void Start()
    {
        okButton.onClick.AddListener(OnOkButtonClick);
    }

    void OnOkButtonClick()
    {
        playerName = nameInputField.text;
        Debug.Log("玩家名字: " + playerName);
        PlayerPrefs.SetString("PlayerName", nameInputField.text);
        basicSpawner.StartGame(basicSpawner.gameMode);
        Destroy(gameObject);
        // 在这里执行您想要进行的下一步操作。
        // 例如：加载新场景、显示游戏开始动画等。
    }
}
