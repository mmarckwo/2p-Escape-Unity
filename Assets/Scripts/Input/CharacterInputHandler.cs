using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;
    bool isUseButtonPressed = false;
    bool isThrowButtonPressed = false;

    LocalCameraHandler localCameraHandler;


    // Start is called before the first frame update
    private void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // view input.
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1; // invert the mouse look.

        // move input.
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        // jump
        if (Input.GetButtonDown("Jump"))
            isJumpButtonPressed = true;

        // use held item. 
        if (Input.GetButtonDown("Fire1"))
            isUseButtonPressed = true;

        // throw held item.
        if (Input.GetButtonDown("Fire2"))
            isThrowButtonPressed = true;

        // set view
        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        // aim data.
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;

        // move data.
        networkInputData.movementInput = moveInputVector;

        // jump data.
        networkInputData.isJumpPressed = isJumpButtonPressed;

        // use item data.
        networkInputData.isUseButtonPressed = isUseButtonPressed;

        // throw item data.
        networkInputData.isThrowButtonPressed = isThrowButtonPressed;

        // reset variables after their states have been read.
        isJumpButtonPressed = false;
        isUseButtonPressed = false;
        isThrowButtonPressed = false;

        return networkInputData;
    }
}
