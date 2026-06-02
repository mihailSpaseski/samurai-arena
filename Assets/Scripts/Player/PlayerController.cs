using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private float rotationSpeed = 10f;

    [Header("Dash")]
    [SerializeField] private SwipeInput swipeInput;
    [SerializeField] private float dashDistance = 4f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;

    private bool isDashing;
    private float dashCooldownTimer;

    private Vector3 moveDirection;

    private void Update()
    {
        HandleDashCooldown();

        if (!isDashing)
        {
            HandleMovement();
        }

        HandleSwipeDash();
    }

    private void HandleDashCooldown()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleSwipeDash()
    {
        if (swipeInput.SwipeDetected && dashCooldownTimer <= 0)
        {
            Vector3 dashDirection = new Vector3(
                swipeInput.SwipeDirection.x,
                0f,
                swipeInput.SwipeDirection.y
            );

            StartCoroutine(Dash(dashDirection));

            swipeInput.ResetSwipe();
        }
    }

    private void HandleMovement()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.magnitude > 1f)
            inputDirection.Normalize();

        Camera mainCamera = Camera.main;

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection =
            cameraForward * inputDirection.z +
            cameraRight * inputDirection.x;

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private System.Collections.IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;

        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition =
            startPosition + direction.normalized * dashDistance;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / dashDuration;

            Vector3 movePosition =
                Vector3.Lerp(startPosition, targetPosition, t);

            characterController.enabled = false;
            transform.position = movePosition;
            characterController.enabled = true;

            yield return null;
        }

        dashCooldownTimer = dashCooldown;

        isDashing = false;
    }
}