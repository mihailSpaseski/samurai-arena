using System.Collections;
using Photon.Pun;
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

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool isDashing;
    private Rigidbody dashHitboxRb;
    private float dashCooldownTimer;
    private DashDamage dashDamage;
    private Vector3 moveDirection;

    // ── Initialisation ─────────────────────────────────

    public void InitialiseLocalPlayer()
    {
        StartCoroutine(InitialiseNextFrame());
    }

    private IEnumerator InitialiseNextFrame()
    {
        yield return null; // wait one frame for scene to fully load

        if (joystick == null)
            joystick = FindFirstObjectByType<FixedJoystick>();

        if (swipeInput == null)
            swipeInput = FindFirstObjectByType<SwipeInput>();

        if (dashIndicator == null)
            dashIndicator = FindFirstObjectByType<DashIndicatorUI>();

        dashHitboxRb = dashHitbox.GetComponent<Rigidbody>();
        dashDamage = dashHitbox.GetComponent<DashDamage>();

        Debug.Log($"Initialised — Joystick: {joystick != null} | SwipeInput: {swipeInput != null} | DashIndicator: {dashIndicator != null}");
    }

    private void Start()
    {
        if (GetComponent<NetworkedPlayer>() == null)
        {
            dashHitboxRb = dashHitbox.GetComponent<Rigidbody>();
            dashDamage = dashHitbox.GetComponent<DashDamage>();
        }
    }

    // ── Update ─────────────────────────────────────────

    private void Update()
    {
        HandleDashCooldown();

        if (!isDashing)
            HandleMovement();

        HandleSwipeDash();
    }

    private void HandleDashCooldown()
    {
        if (dashCooldownTimer <= 0) return;

        dashCooldownTimer -= Time.deltaTime;

        if (dashCooldownTimer < 0)
            dashCooldownTimer = 0;

        if (dashIndicator == null) return;

        if (dashCooldownTimer > 0)
            dashIndicator.SetReadyFalse();
        else
            dashIndicator.SetReadyTrue();

        dashIndicator.SetText(dashCooldownTimer);
    }

    private void HandleSwipeDash()
    {
        if (swipeInput == null) return;
        if (!swipeInput.SwipeDetected) return;

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
        if (joystick == null) return;

        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.magnitude > 1f)
            inputDirection.Normalize();

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

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

        if (animator != null)
            animator.SetFloat("Speed", moveDirection.magnitude);
    }

    // ── Dash ───────────────────────────────────────────

    private IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;

        GetComponent<PlayerHealth>()?.SetInvulnerable(true);

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
            CheckDashHits();
            yield return null;
        }

        characterController.enabled = true;
        dashCooldownTimer = dashCooldown;
        dashHitbox.enabled = false;
        GetComponent<PlayerHealth>()?.SetInvulnerable(false);
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
            if (hit.transform.root.gameObject == transform.root.gameObject)
                continue;

            GameObject target = hit.transform.root.gameObject;

            if (dashDamage.AlreadyHit(target))
                continue;

            dashDamage.RegisterHit(target);

            NetworkedHealth networkedHealth =
                hit.GetComponentInParent<NetworkedHealth>();
            DummyHealth dummy = hit.GetComponentInParent<DummyHealth>();

            if (networkedHealth != null)
            {
                int myViewID = GetComponent<PhotonView>().ViewID;
                networkedHealth.TakeDamage(dashDamage.Damage, myViewID);
                continue;
            }

            dummy?.TakeDamage(dashDamage.Damage);
        }
    }
}