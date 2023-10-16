using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class CharacterMovementHandler : NetworkBehaviour
{
    Vector2 viewInput;
    //rotation
    float cameraRotationX=0;
    float cameraRotationY=0;
    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    Camera localCamera;
    private void Awake()
    {
        networkCharacterControllerPrototypeCustom= GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
    }
    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputDataFPS NetworkInputDataFPS))
        {
            //rotate the view
            print(NetworkInputDataFPS.rotationInput);
            networkCharacterControllerPrototypeCustom.Rotate(NetworkInputDataFPS.rotationInput);
            //move
            Vector3 moveDirection = transform.forward*NetworkInputDataFPS.movementInput.y+transform.right*NetworkInputDataFPS.movementInput.x;
            moveDirection.Normalize();

            networkCharacterControllerPrototypeCustom.Move(moveDirection);
            //jump
            if(NetworkInputDataFPS.isJumpPressed)
                networkCharacterControllerPrototypeCustom.Jump();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraRotationX+= viewInput.y*Time.deltaTime;
        cameraRotationX=Mathf.Clamp(cameraRotationX,-90,90);
        localCamera.transform.localRotation=Quaternion.Euler(cameraRotationX,0,0);
    }
    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput=viewInput;
    }
}
