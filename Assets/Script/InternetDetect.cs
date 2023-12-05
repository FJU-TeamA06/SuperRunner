using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
public class InternetDetect : MonoBehaviour
{
    public NetworkRunner Runner;
    public GameObject NoInternetPanel;
    // Start is called before the first frame update
    void Start()
    {
        
        NoInternetPanel.SetActive(!CheckInternetConnection());

    }
    public void onGoHomeButtonClick()
    {
        if(Runner!=null)
        {
            Runner.Shutdown();
            print("Have Runner,Shutdown");
        }
        SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    void Update()
    {
        NoInternetPanel.SetActive(!CheckInternetConnection());
        Runner = FindObjectOfType<NetworkRunner>(); // 取得 NetworkRunner 的實例
    }
    bool CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Debug.Log("沒有網路連線");
            return false;
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                //Debug.Log("通過行動數據網路連接");
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                //Debug.Log("通過 LAN 或 Wi-Fi 連接");
            }
            return true;
        }
    }
}
