using BepInEx;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

[BepInPlugin("frl.internal.cool.wasd", "frlwasd", "1.0.0")]
public class WASDMovement : BaseUnityPlugin
{
    private const float MoveSpeed = 10f;
    private const float RotationSpeed = 0.5f;
    private const float Gravity = 9.81f;
    private Camera playerCamera;
    private Rigidbody playerRb;
    private float yaw;
    private float pitch;
    private const float MinPitch = -80f;
    private const float MaxPitch = 80f;

    void Start()
    {
        playerCamera = Camera.main;
        playerRb = GTPlayer.Instance.GetComponent<Rigidbody>();
        if (playerRb == null)
            playerRb = GTPlayer.Instance.AddComponent<Rigidbody>();

        Vector3 angles = playerCamera.transform.eulerAngles;
        pitch = angles.x > 180 ? angles.x - 360 : angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        Transform playerTransform = GTPlayer.Instance.transform;
        if (playerRb == null)
        {
            playerRb = GTPlayer.Instance.GetComponent<Rigidbody>();
            playerRb.velocity = Vector3.zero;
        }

        HandleCameraRotation();

        if (!UnityInput.Current.GetKey(KeyCode.Space))
            playerRb.velocity += Vector3.down * Gravity * Time.deltaTime;
        else
            playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);

        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = Vector3.zero;

        if (UnityInput.Current.GetKey(KeyCode.W))
            moveDirection += forward;
        if (UnityInput.Current.GetKey(KeyCode.S))
            moveDirection -= forward;
        if (UnityInput.Current.GetKey(KeyCode.A))
            moveDirection -= right;
        if (UnityInput.Current.GetKey(KeyCode.D))
            moveDirection += right;

        if (UnityInput.Current.GetKey(KeyCode.Space))
            moveDirection += Vector3.up;
        if (UnityInput.Current.GetKey(KeyCode.LeftShift))
            moveDirection -= Vector3.up;

        if (moveDirection.magnitude > 0)
        {
            moveDirection.Normalize();
            playerTransform.position += moveDirection * Time.deltaTime * MoveSpeed;
        }
    }

    private void HandleCameraRotation()
    {
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yaw += mouseDelta.x * RotationSpeed;
            pitch -= mouseDelta.y * RotationSpeed;

            pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);

            if (yaw > 180f)
                yaw -= 360f;
            if (yaw < -180f)
                yaw += 360f;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }


        playerCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}