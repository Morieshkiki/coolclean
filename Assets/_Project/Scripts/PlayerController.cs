using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool jumpQueued = false;
    private bool isRunning = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // --- CALLED BY PLAYERINPUT EVENTS ---
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        // "performed" is true while held, "canceled" is when released
        isRunning = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log($"OnJump called. isGrounded: {controller.isGrounded}, performed: {context.performed}");
        if (context.performed && controller.isGrounded)
        {
            jumpQueued = true;
            Debug.Log("Jump queued!");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Interact pressed!");
        }
    }

    void Update()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // 1. Calculate movement input (X/Z only, Y is vertical for gravity/jump)
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        if (move.magnitude > 1f)
            move.Normalize();

        // 2. Rotate character to face movement direction (optional for top-down)
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 720f * Time.deltaTime);
        }

        // 3. Handle gravity and jump
        if (controller.isGrounded)
        {
            velocity.y = -2f; // Small negative value to keep grounded

            if (jumpQueued)
            {
                velocity.y = jumpForce;
                jumpQueued = false;
                Debug.Log("Jump!");
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // 4. Combine movement with gravity/jump
        Vector3 totalMove = move * currentSpeed;
        totalMove.y = velocity.y;

        // 5. Apply movement (call only ONCE)
        controller.Move(totalMove * Time.deltaTime);
    }
}
