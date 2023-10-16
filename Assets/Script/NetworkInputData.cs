using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum InputButtonsFPS
{
    JUMP,
    FIRE
}



public struct NetworkInputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector3 aimForwardVector;
    public Vector3 movementInput;
}