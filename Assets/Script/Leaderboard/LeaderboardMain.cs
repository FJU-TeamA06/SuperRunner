using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Newtonsoft.Json; // Make sure you have Newtonsoft.Json package

public class LeaderboardMain : MonoBehaviour
{
    public GameObject LeaderboardTextObject;

    IEnumerator DoRequest()
    {
        TextMeshProUGUI leaderboardText = LeaderboardTextObject.GetComponent<TextMeshProUGUI>();
        leaderboardText.text = "Loading...";
        var request = UnityWebRequest.Get("http://140.136.151.71:5000/leaderboard?mode=getleaderscore");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            yield break;
        }

        var json = request.downloadHandler.text;
        Debug.Log(json);

        // Parse the JSON data
        var leaderboardData = JsonConvert.DeserializeObject<List<List<object>>>(json);
        var formattedText = FormatLeaderboardData(leaderboardData);
        leaderboardText.text = formattedText;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DoRequest());
    }
    public void OnRefreshButtonClick()
    {
        StartCoroutine(DoRequest());
    }
    string FormatLeaderboardData(List<List<object>> data)
    {
        string formatted = "";
        foreach (var item in data)
        {
            string period = item[0].ToString();
            string identifier = item[1].ToString();
            int score = Convert.ToInt32(item[2]);

            switch (period)
            {
                case "day":
                    period = "當日最高分";
                    break;
                case "week":
                    period = "當週最高分";
                    break;
                case "month":
                    period = "當月最高分";
                    break;
            }

            formatted += $"{period} - {identifier}: {score}\n";
        }
        return formatted;
    }


}
