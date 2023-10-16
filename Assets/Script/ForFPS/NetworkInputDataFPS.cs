using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum InputButtons
{
    JUMP,
    FIRE
}



public struct NetworkInputDataFPS : INetworkInput
{

    public NetworkButtons buttons;
    public NetworkBool isJumpPressed;
    public float rotationInput;
    public Vector3 aimForwardVector;
    public Vector2 movementInput;
}