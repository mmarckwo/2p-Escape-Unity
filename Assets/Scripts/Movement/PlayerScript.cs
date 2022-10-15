using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class PlayerScript : NetworkBehaviour
{
    private CharacterController _originalController;
    private NetworkCharacterControllerPrototype _networkController;
    public float originalStepOffset;

    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    Camera localCamera;

    [Header("Physics")]
    public float jumpForce = 22.5f;
    public float speed = 10.0f;
    [HideInInspector]
    Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f); // How fast we are going.
    // player will use its own gravity for jump physics.
    public float gravityScale = 2.5f;
    public static float globalGravity = -9.81f;
    [HideInInspector]
    public int umbrellaFloat = 1;

    [Header("Jump Sound")]
    public AudioSource jumpSound;

    void Awake()
    { 
        // don't need these?
        _originalController = GetComponent<CharacterController>();
        //_networkController = GetComponent<NetworkCharacterControllerPrototype>();

        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();

        localCamera = GetComponentInChildren<Camera>();

        originalStepOffset = _originalController.stepOffset;
    }

    public override void FixedUpdateNetwork()
    {
        // get input from the network
        if (GetInput(out NetworkInputData networkInputData))
        {
            // rotate the transform according to the client aim vector.
            transform.forward = networkInputData.aimForwardVector;

            // cancel out x axis rotation to prevent tilting on character model.
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            // move.
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            networkCharacterControllerPrototypeCustom.Move(moveDirection);

            // jump
            if (networkInputData.isJumpPressed)
                networkCharacterControllerPrototypeCustom.Jump();
        }
    }

    public void UmbrellaFall()
    {
        networkCharacterControllerPrototypeCustom.UmbrellaFall();
    }

    void Jump()
    {

        // if the umbrella is open, greatly reduce jump height. 
        if (umbrellaFloat != 1)
        {
            velocity.y = Mathf.Sqrt((jumpForce / 4f) * -2f * globalGravity / umbrellaFloat);
        }
        else
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * globalGravity);
        }
        
    }

}
