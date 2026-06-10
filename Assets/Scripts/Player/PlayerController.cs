using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private BoxCollider dashHitbox;

    [Header("References")]
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private float rotationSpeed = 10f;

    [Header("Dash")]
    [SerializeField] private SwipeInput swipeInput;
    [SerializeField] private float dashDistance = 4f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;

    [SerializeField] private DashIndicatorUI dashIndicator;

    [Header("Effects")]
    [SerializeField] private GameObject dashTrailPrefab;

    private bool isDashing;
    private Rigidbody dashHitboxRb;
    private float dashCooldownTimer;
    private DashDamage dashDamage;


    private Vector3 moveDirection;

    public void InitialiseLocalPlayer()
    {
        if (joystick == null)
            joystick = FindFirstObjectByType<FixedJoystick>();

        if (swipeInput == null)
            swipeInput = FindFirstObjectByType<SwipeInput>();

        if (dashIndicator == null)
            dashIndicator = FindFirstObjectByType<DashIndicatorUI>();

        dashHitboxRb = dashHitbox.GetComponent<Rigidbody>();
        dashDamage = dashHitbox.GetComponent<DashDamage>();
    }

    private void Start()
    {
        // only self-initialise if not controlled by NetworkedPlayer
        if (GetComponent<NetworkedPlayer>() == null)
        {
            dashHitboxRb = dashHitbox.GetComponent<Rigidbody>();
            dashDamage = dashHitbox.GetComponent<DashDamage>();
        }
    }

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

        if (dashCooldownTimer >= 0.0)
        {
            dashCooldownTimer -= Time.deltaTime;

            if (dashCooldownTimer < 0.0)
            {
                dashCooldownTimer = 0;
            }

            if (dashCooldownTimer > 0.0)
            {
                dashIndicator.SetReadyFalse();
            }
            else
            {
                dashCooldownTimer = 0;
                dashIndicator.SetReadyTrue();
            }
            dashIndicator.SetText(dashCooldownTimer);
        }
    }

    private void HandleSwipeDash()
    {
        if (!swipeInput.SwipeDetected)
            return;

        // Ignore swipe completely during cooldown
        if (dashCooldownTimer > 0 || isDashing)
        {
            swipeInput.ResetSwipe();
            return;
        }

        Vector3 dashDirection = new Vector3(
            swipeInput.SwipeDirection.x,
            0f,
            swipeInput.SwipeDirection.y
        );

        StartCoroutine(Dash(dashDirection));

        swipeInput.ResetSwipe();
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
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        dashDamage.ResetHits();
        dashHitbox.enabled = true;

        if (dashTrailPrefab != null)
            Instantiate(dashTrailPrefab, transform.position, transform.rotation);

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction.normalized * dashDistance;

        characterController.enabled = false;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dashDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            dashHitboxRb.MovePosition(transform.position);

            // check for hits each frame using overlap
            CheckDashHits();

            yield return null;
        }

        characterController.enabled = true;
        dashCooldownTimer = dashCooldown;
        dashHitbox.enabled = false;
        isDashing = false;
    }

    private void CheckDashHits()
    {
        Collider[] hits = Physics.OverlapBox(
            transform.position,
            new Vector3(1.5f, 1f, 1.5f),
            transform.rotation,
            Physics.AllLayers,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider hit in hits)
        {
            // compare actual GameObject instance not name
            if (hit.transform.root.gameObject == transform.root.gameObject)
                continue;

            GameObject target = hit.transform.root.gameObject;


            if (dashDamage.AlreadyHit(target))
                continue;

            dashDamage.RegisterHit(target);

            NetworkedHealth networkedHealth =
                hit.GetComponentInParent<NetworkedHealth>();
            DummyHealth dummy = hit.GetComponentInParent<DummyHealth>();

            Debug.Log($"Valid hit on: {hit.transform.root.name} | NetworkedHealth: {networkedHealth != null}");

            if (networkedHealth != null)
            {
                networkedHealth.TakeDamage(dashDamage.Damage);
                continue;
            }

            dummy?.TakeDamage(dashDamage.Damage);
        }
    }
}