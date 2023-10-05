using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;

public class FinishPlane : MonoBehaviour
{
    public GameObject finishPanel;
 
    void Start()
    {
        
    }

    public void FinishClick()
    {
        finishPanel.SetActive(true);
    }
}
