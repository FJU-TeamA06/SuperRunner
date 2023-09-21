using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera MainCamera;
    public Camera SideCamera;
    public Camera FirstCamera;

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
        GameObject FirstCameraObject = GameObject.FindGameObjectWithTag("FirstCamera");
        if (sideCameraObject != null)
        {
            FirstCamera = FirstCameraObject.GetComponent<Camera>();
        }
        else
        {
            Debug.LogWarning("FirstCamera object not found or not active.");
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
        if (FirstCamera.enabled)
        {
            transform.LookAt(transform.position + FirstCamera.transform.rotation * Vector3.forward,
                FirstCamera.transform.rotation * Vector3.up);
        }
    }
}
