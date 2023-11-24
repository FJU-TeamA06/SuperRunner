using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine.SceneManagement;
public class FinishPlane : MonoBehaviour
{
    private BasicSpawner basicSpawner;  //引用
    public GameObject finishPanel;
    public GameObject ButtonNextObject;
    private Button buttonNext;
    public NetworkRunner Runner;
    void Awake()
    {
        basicSpawner = FindObjectOfType<BasicSpawner>(); // 取得 BasicSpawner 的實例
        Runner = FindObjectOfType<NetworkRunner>(); // 取得 NetworkRunner 的實例
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        finishPanel.SetActive(false);
        buttonNext = ButtonNextObject.GetComponent<Button>();
        if(basicSpawner.levelIndex==3)
        {
            TextMeshProUGUI buttonNextText = buttonNext.GetComponentInChildren<TextMeshProUGUI>();
            buttonNextText.text="回首頁";
        }
        
    }
    public void Finish3Click() //FPS
    {
        finishPanel.SetActive(true);
    }

    public void FinishClick() //level 1 & 2
    {
        finishPanel.SetActive(true);
        //StartCoroutine(DeactivateAfterDelay(3.0f)); // �Ұʨ�{����5�����������O
    }


    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // ���ݫ��w������
        if(basicSpawner.levelIndex==3)
        {
            TextMeshProUGUI buttonNextText = buttonNext.GetComponentInChildren<TextMeshProUGUI>();
            buttonNext.interactable = true;
            buttonNextText.text="回首頁";
        }
        finishPanel.SetActive(false); // �������O

    }
    public void OnButtonNextClick()
    {
        TextMeshProUGUI buttonNextText = buttonNext.GetComponentInChildren<TextMeshProUGUI>();
        if(buttonNextText.text=="回首頁")
        {
            Runner.Shutdown();
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(DeactivateAfterDelay(1.0f));
            buttonNext.interactable = false;
        }
        
    }
}
