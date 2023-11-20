using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;

public class StartButton : MonoBehaviour
{
    //public Button okButton;
    public GameObject levelPanel; 
    public GameObject SettingButton;
    public GameObject LeaderboardButton;
    

    public void OnButtonClick()
    {
        levelPanel.SetActive(true); 
        SettingButton.SetActive(false);
        LeaderboardButton.SetActive(false);
    }
}
