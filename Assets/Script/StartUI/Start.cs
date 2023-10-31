using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public GameObject loadingPanel; 
    public void playgame()
    {
        loadingPanel.SetActive(true); 
        SceneManager.LoadScene(1);
    }
}
