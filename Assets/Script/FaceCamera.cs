using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera MainCamera;
    public Camera SideCamera;
    
    void Start()
    {
        GameObject MainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (MainCameraObject != null)
        {
            MainCamera = MainCameraObject.GetComponent<Camera>();
        }
        else
        {
            Debug.LogWarning("MainCamera object not found or not active.");
        }
        GameObject sideCameraObject = GameObject.FindGameObjectWithTag("SideCamera");
        if (sideCameraObject != null)
        {
            SideCamera = sideCameraObject.GetComponent<Camera>();
        }
        else
        {
            Debug.LogWarning("SideCamera object not found or not active.");
        }
    }

    void Update()
    {
        
        if(MainCamera.enabled)
        {
            transform.LookAt(transform.position + MainCamera.transform.rotation * Vector3.forward,
                MainCamera.transform.rotation * Vector3.up);
        }
        if(SideCamera.enabled)
        {
            transform.LookAt(transform.position + SideCamera.transform.rotation * Vector3.forward,
                SideCamera.transform.rotation * Vector3.up);
        }
    }
}
