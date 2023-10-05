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

    

    public void OnButtonClick()
    {
        levelPanel.SetActive(true); 
    }
}
