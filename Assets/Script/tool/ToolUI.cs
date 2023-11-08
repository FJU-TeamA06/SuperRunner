using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fusion;
using Fusion.Sockets;


public class ToolUI : MonoBehaviour
{
    public GameObject toolPanel;
    public Text tooltext;

    void Start()
    {
        toolPanel.SetActive(false);
        //tooltext.SetActive(false);
    }
    public void toolUI()
    {
        StartCoroutine(DeleteAfterDelay(5.0f)); // 啟動協程等待5秒後關閉面板

    }

    private IEnumerator DeleteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 等待指定的秒數

        toolPanel.SetActive(false); // 關閉面板
        //tooltext.SetActive(false);
    }

}
