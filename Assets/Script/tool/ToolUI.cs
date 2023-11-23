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
        //toolPanel.SetActive(false);
        //tooltext.SetActive(false);
    }
    public void toolUI()
    {
        //StartCoroutine(DeleteAfterDelay(5.0f)); // �Ұʨ�{����5�����������O

    }

    /*private IEnumerator DeleteAfterDelay(float delay)
    {
        //yield return new WaitForSeconds(delay); // ���ݫ��w������
        
        //toolPanel.SetActive(false); // �������O

        //tooltext.SetActive(false);
    }*/

}
