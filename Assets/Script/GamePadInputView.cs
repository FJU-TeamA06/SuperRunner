using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class GamePadInputView : MonoBehaviour
{
    // Start is called before the first frame update
    public InputActionAsset myActions;
    public RawImage ObjectDpadUpImage;
    public RawImage ObjectDpadDownImage;
    public RawImage ObjectDpadLeftImage;
    public RawImage ObjectDpadRightImage;
    public RawImage ObjectButtonAImage;
    public RawImage ObjectButtonBImage;
    public RawImage ObjectButtonStartImage;
    public RawImage ObjectButtonSelectImage;
    public RawImage ObjectDot;
    public float Deadzone=0f;
    public float Sensitivity=47f;
    public Vector2 center = new Vector2(163.6f, -53.4f); 

    public float radius = 47f;

    Vector2 position;
    void Start()
    {
        myActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        InputAction Up = myActions.FindAction("Up");
        InputAction Down = myActions.FindAction("Down");
        InputAction Left = myActions.FindAction("Left");
        InputAction Right = myActions.FindAction("Right");
        InputAction JUMP = myActions.FindAction("A");
        InputAction Fire = myActions.FindAction("B");
        InputAction View = myActions.FindAction("View");
        InputAction Start = myActions.FindAction("Start");
        InputAction RightPitch = myActions.FindAction("RightPitch");
        InputAction RightYaw = myActions.FindAction("RightYaw");
        if (Up.ReadValue<float>()>0.5f){
        ObjectDpadUpImage.enabled=true;
        }else{
        ObjectDpadUpImage.enabled=false;
        }

        if (Down.ReadValue<float>()>0.5f){
        ObjectDpadDownImage.enabled=true;  
        }else{
        ObjectDpadDownImage.enabled=false;
        }

        if (Left.ReadValue<float>()>0.5f){
        ObjectDpadLeftImage.enabled=true;
        }else{
        ObjectDpadLeftImage.enabled=false;  
        }

        if (Right.ReadValue<float>()>0.5f){
        ObjectDpadRightImage.enabled=true;
        }else{
        ObjectDpadRightImage.enabled=false;
        }      
        if (JUMP.ReadValue<float>()>0.5f){
        ObjectButtonAImage.enabled=true;
        }else{
        ObjectButtonAImage.enabled=false;
        } 
        if (Fire.ReadValue<float>()>0.5f){
        ObjectButtonBImage.enabled=true;
        }else{
        ObjectButtonBImage.enabled=false;
        }               
        if (Start.ReadValue<float>()>0.5f){
        ObjectButtonStartImage.enabled=true;
        }else{
        ObjectButtonStartImage.enabled=false;
        } 
        if (View.ReadValue<float>()>0.5f){
        ObjectButtonSelectImage.enabled=true;
        }else{
        ObjectButtonSelectImage.enabled=false;
        }     
        float vertical = RightPitch.ReadValue<float>();
        float horizontal = RightYaw.ReadValue<float>();

        if (Mathf.Abs(vertical) < Deadzone) vertical = 0;
        if (Mathf.Abs(horizontal) < Deadzone) horizontal = 0;

        position.x = horizontal * Sensitivity; 
        position.y = vertical * Sensitivity;

        //限制位置不要超出範圍
        position.x = Mathf.Clamp(position.x, -47, 47)+163.6f;
        position.y = Mathf.Clamp(position.y, -47, 47)+-53.4f;
        Vector2 direction = position - center;
        if(direction.magnitude > radius) {
        //如果距離超出半徑,則將方向歸一化並乘上半徑
        direction = direction.normalized * radius;

        //更新位置為限制在圓內的位置
        position = center + direction; 
        }
        //將位置設置到小點物件
        ObjectDot.transform.localPosition = position;

    }
}
