using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FirstCamera : MonoBehaviour
{
    private Vector3 offset;
    public float sensitivity = 1.0f; // 控制視角靈敏度
    public Transform playerBody; // 用於旋轉角色身體的變換
    //public Transform player; // 玩家的Transform

    private float rotationX = 0;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // 鎖定滑鼠游標到遊戲視窗內
        //offset = transform.position - playerBody.transform.position;
        //transform.localRotation = Quaternion.identity;
        //transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
    }

    void Update()
    {
        // 偵測是否按住滑鼠右鍵（您可以更改成其他按鍵）
        if (Input.GetMouseButton(1))
        {
            
            // 獲取滑鼠輸入
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            // 根據滑鼠輸入旋轉視角攝像機
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -30f, 30f); // 限制上下旋轉的角度
            transform.localRotation = Quaternion.Euler(rotationX, 90, 0); // 上下旋轉
            playerBody.Rotate(Vector3.up * mouseX); // 左右旋轉
        }
    }
  
}