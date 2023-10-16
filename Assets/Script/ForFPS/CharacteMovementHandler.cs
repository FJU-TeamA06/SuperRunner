using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class CharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    private void Awake()
    {
        networkCharacterControllerPrototypeCustom= GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }
    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData NetworkInputDataFPS))
        {
            //move
            Vector3 moveDirection = transform.forward*NetworkInputDataFPS.movementInput.y+transform.right*NetworkInputDataFPS.movementInput.x;
            moveDirection.Normalize();

            networkCharacterControllerPrototypeCustom.Move(moveDirection);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
