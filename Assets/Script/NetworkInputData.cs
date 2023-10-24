using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum InputButtons
{
    JUMP,
    FIRE,
}
public enum InputMode
{
    ModeA,
    ModeFPS
}



public struct NetworkInputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector2        MoveInput;
    public Angle          Pitch;
    public Angle          Yaw;
}