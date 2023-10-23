using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;

public class FinishPlane : MonoBehaviour
{
    public GameObject finishPanel;
    public Transform teleportDestination; // 設定要傳送到的目標位置

    void Start()
    {

    }

    public void FinishClick()
    {
        finishPanel.SetActive(true);
        StartCoroutine(DeactivateAfterDelay(5.0f)); // 啟動協程等待5秒後關閉面板
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 等待指定的秒數

        finishPanel.SetActive(false); // 關閉面板

        // 檢查是否設定了傳送目標位置
        if (teleportDestination != null)
        {
            // 獲取所有擁有"Player"標籤的物體
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                // 將玩家傳送到指定位置
                player.transform.position = teleportDestination.position;
            }
        }
    }
}
