using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    //public const int MOUSEBUTTON1 = 1;
    //public const int MOUSEBUTTON2 = 2;

    //public const int BUTTON_FORWARD = 3;
    //public const int BUTTON_BACKWARD = 4;
    //public const int BUTTON_LEFT = 5;
    //public const int BUTTON_RIGHT = 6;

    //public byte buttons;
    public Vector3 direction;

    public Vector2 movementInput;
    public Vector3 aimForwardVector;
    public NetworkBool isJumpPressed;
    public NetworkBool isUseButtonPressed;
    public NetworkBool isThrowButtonPressed;
    public NetworkBool isFirstItemButtonPressed;
    public NetworkBool isSecondItemButtonPressed;
}
