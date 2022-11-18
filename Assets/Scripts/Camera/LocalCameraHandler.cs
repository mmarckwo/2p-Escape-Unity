using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;

    // input
    Vector2 viewInput;

    // rotation
    float cameraRotationX = 0;
    float cameraRotationY = 0;

    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    Camera localCamera;

    private void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }

    void Start()
    {
        // detach camera if enabled.
        if (localCamera.enabled)
            localCamera.transform.parent = null;
    }

    void LateUpdate()
    {
        if (cameraAnchorPoint == null)
            return;

        if (!localCamera.enabled)
            return;

        localCamera.transform.position = cameraAnchorPoint.position;

        // calculate rotation
        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerPrototypeCustom.viewYRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -89.9f, 89.9f);

        cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterControllerPrototypeCustom.rotationSpeed;

        // apply rotation
        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
