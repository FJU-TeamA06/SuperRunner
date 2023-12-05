using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ExitButtonOnClick()
    {
        // 退出遊戲
        Application.Quit();

        // 以下代碼是為了在 Unity 編輯器中測試
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
