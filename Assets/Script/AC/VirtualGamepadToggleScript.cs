using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualGamepadToggleScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject VirtualGamepad;
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        if(Application.isMobilePlatform)
        {
            VirtualGamepad.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnButtonClick()
    {
        VirtualGamepad.SetActive(!VirtualGamepad.activeSelf);
    }
}
