using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 15f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private InputDevice assignedDevice;

    public void Initialize(InputDevice device)
    {
        assignedDevice = device;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (assignedDevice == null)
            return;

        // Read input from the assigned device
        if (assignedDevice is Gamepad gamepad)
        {
            moveInput = gamepad.leftStick.ReadValue();
            moveInput = moveInput.normalized;
        }
        else if (assignedDevice is Keyboard)
        {
            moveInput = new Vector2(
                (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
                (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0)
            );
            moveInput = moveInput.normalized;
        }
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.linearVelocity = move;
    }

    public InputDevice GetAssignedDevice()
    {
        return assignedDevice;
    }
}