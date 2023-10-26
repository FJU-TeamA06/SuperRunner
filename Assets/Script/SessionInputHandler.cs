using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;

public class SessionInputHandler : MonoBehaviour
{
    
    public InputField sessionInputField;
    public Button okButton;
    public GameObject levelSelectionPanel; // 添加一个新的游戏对象，用于显示关卡选择界面
    public GameObject sessionInputPanel; // 添加一个新的游戏对象，用于表示名字输入面板
    // Start is called before the first frame update
    void Start()
    {
        okButton.onClick.AddListener(OnOkButtonClick);
    }
    void OnOkButtonClick()
    {
        Debug.Log("房間名字: " + sessionInputField.text);
        PlayerPrefs.SetString("SessionName", sessionInputField.text);
        levelSelectionPanel.SetActive(true); // 显示关卡选择界面
        sessionInputPanel.SetActive(false); // 隐藏名字输入面板
        // 将下面这行代码移动到新的方法中，当用户选择关卡后才开始游戏
        // basicSpawner.StartGame(basicSpawner.gameMode);
    }
    // Update is called once per frame
}
