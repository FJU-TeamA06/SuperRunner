using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingExitScript : MonoBehaviour
{
    public void OnButtonClick()
    {
        SceneManager.LoadScene(0);
    }
}
