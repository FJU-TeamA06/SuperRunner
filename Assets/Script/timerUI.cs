using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class timerUI : MonoBehaviour
{
    public Text timerText;
    private float timeElapsed;
    private bool timerActive;
    public Text sessionName;
    public NetworkRunner Runner;
    void Start()
    {
        timerActive = false;
        ResetTimer();
        Runner = FindObjectOfType<NetworkRunner>(); // 取得 NetworkRunner 的實例
        if(Runner.IsClient)
        {
            sessionName.text="C_房間名稱: "+PlayerPrefs.GetString("SessionName");
        }
        else
        {
            sessionName.text="H_房間名稱: "+PlayerPrefs.GetString("SessionName");
        }
        
    }

    void Update()
    {
        if (timerActive)
        {
            timeElapsed += Time.deltaTime;
            //timerText.text = FormatTime(timeElapsed);
        }
    }

    public void StartTimer()
    {
        //timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public void ResetTimer()
    {
        timeElapsed = 0f;
        //timerText.text = FormatTime(timeElapsed);
    }

    //private string FormatTime(float timeToFormat)
    //{
        //int minutes = (int) timeToFormat / 60;
       // int seconds = (int) timeToFormat % 60;
      //  float fraction = (timeToFormat * 10) % 10;

       // return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
    //}
}
