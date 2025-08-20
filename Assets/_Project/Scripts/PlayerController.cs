using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    private CharacterController controller;
    private Vector2 moveInput;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Hooked from PlayerInput (Invoke Unity Events) → Player/Move
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void Update()
    {
        // Get camera-relative directions on the XZ plane
        var cam = Camera.main;
        if (cam == null) return;

        Vector3 forward = cam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cam.transform.right;
        right.y = 0f;
        right.Normalize();

        // Convert 2D input to world space using camera axes
        Vector3 moveWorld = (right * moveInput.x + forward * moveInput.y);

        if (moveWorld.sqrMagnitude > 0.0001f)
        {
            // Face movement direction
            Quaternion targetRot = Quaternion.LookRotation(moveWorld, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        controller.SimpleMove(moveWorld * moveSpeed);
    }
}
