using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class timerUI : MonoBehaviour
{
    public Text timerText;
    private float timeElapsed;
    private bool timerActive;
    

    void Start()
    {
        timerActive = false;
        ResetTimer();
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
