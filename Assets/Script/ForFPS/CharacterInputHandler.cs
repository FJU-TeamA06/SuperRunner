using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    bool isJumpButtonPressed = false;
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    //Other components
    CharacterMovementHandler characterMovementHandler;
    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
    }

    // Update is called once per frame
    void Update()
    {
        //view input
        viewInputVector.x=Input.GetAxis("Mouse X");
        viewInputVector.y=Input.GetAxis("Mouse Y")*-1;
        characterMovementHandler.SetViewInputVector(viewInputVector);
        //move input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");
        isJumpButtonPressed=Input.GetButtonDown("Jump");
    }
    public NetworkInputDataFPS GetNetworkInput()
    {

        
        NetworkInputDataFPS networkInputDataFPS = new NetworkInputDataFPS();
        //view data
        networkInputDataFPS.rotationInput=viewInputVector.x;
        //move data
        networkInputDataFPS.movementInput= moveInputVector;
        //jump data
        networkInputDataFPS.isJumpPressed=isJumpButtonPressed;
        return networkInputDataFPS;
    }
}
