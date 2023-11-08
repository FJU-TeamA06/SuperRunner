using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
public class SettingButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        SceneManager.LoadScene(2);
    }
}
