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
    public Vector3 aimForwardVector;
    public Vector3 movementInput;
}