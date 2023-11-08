using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingScript : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField UsernameInputField;
    public TMP_InputField RoomInputField;
    private string playerName = "";
    private string roomName = "";

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName");
        Debug.Log("玩家名字讀檔: " + playerName);
        roomName = PlayerPrefs.GetString("SessionName");
        Debug.Log("房間名字讀檔: " + roomName);
        UsernameInputField.text=playerName;
        RoomInputField.text=roomName;
    }
    public void OnSaveButtonClick()
    {
        playerName = UsernameInputField.text;
        roomName=RoomInputField.text;
        Debug.Log("玩家名字存檔: " + playerName);
        PlayerPrefs.SetString("PlayerName",playerName);
        Debug.Log("房間名字存檔: " + roomName);
        PlayerPrefs.SetString("SessionName",roomName);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
