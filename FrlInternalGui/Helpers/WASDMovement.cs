using BepInEx;
using UnityEngine;
using UnityEngine.InputSystem;
using Player = GorillaLocomotion.Player;

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
        playerRb = Player.Instance.GetComponent<Rigidbody>();

        Vector3 angles = playerCamera.transform.localEulerAngles;

        pitch = angles.x > 180 ? angles.x - 360 : angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        Transform playerTransform = Player.Instance.transform;

        Transform bodyColliderTransform = Player.Instance.bodyCollider.transform;

        if (!UnityInput.Current.GetKey(KeyCode.Space))
            playerRb.velocity += Vector3.down * Gravity * Time.deltaTime;

        if (UnityInput.Current.GetKey(KeyCode.Space))
            playerTransform.position += bodyColliderTransform.up * Time.deltaTime * MoveSpeed;
        if (UnityInput.Current.GetKey(KeyCode.LeftShift))
            playerTransform.position -= bodyColliderTransform.up * Time.deltaTime * MoveSpeed;
        if (UnityInput.Current.GetKey(KeyCode.W))
            playerTransform.position += bodyColliderTransform.forward * Time.deltaTime * MoveSpeed;
        if (UnityInput.Current.GetKey(KeyCode.S))
            playerTransform.position -= bodyColliderTransform.forward * Time.deltaTime * MoveSpeed;
        if (UnityInput.Current.GetKey(KeyCode.A))
            playerTransform.position -= bodyColliderTransform.right * Time.deltaTime * MoveSpeed;
        if (UnityInput.Current.GetKey(KeyCode.D))
            playerTransform.position += bodyColliderTransform.right * Time.deltaTime * MoveSpeed;

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

            playerCamera.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}