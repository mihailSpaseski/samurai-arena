using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastMoveSpeed = 25f;
    [SerializeField] private float rotationSpeed = 0.2f;

    private bool isActive = false;
    private float yaw = 0f;
    private float pitch = 0f;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void EnableSpectator()
    {
        isActive = true;
        transform.position = new Vector3(0, 10, 0);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!isActive) return;

        // rotation — touch/mouse
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -20f, 20f);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // movement
        float speed = Input.GetKey(KeyCode.LeftShift) 
            ? fastMoveSpeed : moveSpeed;

        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"),
            0f,
            Input.GetAxis("Vertical")
        );

        // up/down
        if (Input.GetKey(KeyCode.E)) move.y = 0.1f;
        if (Input.GetKey(KeyCode.Q)) move.y = -0.1f;

        transform.position += transform.TransformDirection(move) 
            * speed * Time.deltaTime;
    }
}