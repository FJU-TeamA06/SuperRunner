using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardMain : MonoBehaviour
{
    IEnumerator DoRequest()
    {
        Debug.Log(WWW.EscapeURL("Testing 1,2,3"));
        var request = UnityWebRequest.Get("http://140.136.151.71:5000/players?mode=getalldata");
        
        yield return request.Send();
        
        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            yield break;
        }
        
        var html = request.downloadHandler.text;
        Debug.Log(html);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DoRequest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
