using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualGamepadToggleScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject VirtualGamepad;
    private Button button;
    private EventSystem eventSystem;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        gameObject.SetActive(Application.isMobilePlatform);
        eventSystem = FindObjectOfType<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnButtonClick()
    {
        VirtualGamepad.SetActive(!VirtualGamepad.activeSelf);
        eventSystem.SetSelectedGameObject(null);
    }
}
