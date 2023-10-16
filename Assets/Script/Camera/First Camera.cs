using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class FirstCamera : MonoBehaviour
{
    Vector2 viewInput;
    float cameraRotationX = 0;
    float cameraRotationY = 0;
    NetworkCharacterControllerPrototype networkCharacterControllerPrototypeCustom;
    private Vector3 offset;
    public Transform cameraAnchorPoint; 
    public float sensitivity = 1.0f; // 控制視角靈敏度
    public Transform playerBody; // 用於旋轉角色身體的變換
    //public Transform player; // 玩家的Transform
    private float rotationX = 0;
    private float rotationY = 0;
    Camera localCamera;

    
    private void Awake()
    {
        localCamera = GetComponent<Camera>();
    }
    void Start()
    {
        if(localCamera.enabled)
                localCamera.transform.parent=null;
        if (SceneManager.GetActiveScene().name == "FPS")
        {
            Cursor.lockState = CursorLockMode.Locked; 

        }
            
        
    }

    void LateUpdate()
    {
        if(cameraAnchorPoint==null)
            return;
        if(!localCamera.enabled)
            return;
        localCamera.transform.position = cameraAnchorPoint.position;
        cameraRotationX+=viewInput.y*Time.deltaTime;
        cameraRotationX=Mathf.Clamp(cameraRotationX,-90,90);
        cameraRotationY+=viewInput.x*Time.deltaTime;
        localCamera.transform.rotation=Quaternion.Euler(cameraRotationX,cameraRotationY,0);
        // 偵測是否按住滑鼠右鍵（您可以更改成其他按鍵）
        //if (Input.GetMouseButton(1))
        //{
/*
            // 獲取滑鼠輸入
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            // 根據滑鼠輸入旋轉視角攝像機
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -30f, 30f); // 限制上下旋轉的角度
            rotationY -=mouseX;
            transform.localRotation = Quaternion.Euler(rotationX, 1, 0); // 上下旋轉
            playerBody.Rotate(Vector3.up * mouseX*1); // 左右旋轉
        //}
*/


    }
    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput=viewInput;
    }
  
}