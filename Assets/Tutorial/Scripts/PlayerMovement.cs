using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public bool allowJump = true;

    [Header("Input Keys")]
    public KeyCode jumpKey = KeyCode.Space;
    public string horizontalAxis = "Horizontal"; // 默认 A/D 或 ←/→
    public string verticalAxis = "Vertical";     // 默认 W/S 或 ↑/↓

    private bool allowZMovement = false;
    private bool isGrounded = true;

    private Rigidbody rb;
    private Vector3 inputDirection;

    [Header("Camera Switcher Reference")]
    public CameraSwitcher cameraSwitcher;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 自动读取 CameraSwitcher 中的视角状态
        if (cameraSwitcher != null)
        {
            allowZMovement = cameraSwitcher.isThreeD;
        }

        float x = Input.GetAxisRaw(horizontalAxis);
        float z = allowZMovement ? Input.GetAxisRaw(verticalAxis) : 0f;

        inputDirection = new Vector3(x, 0f, z).normalized;

        // 跳跃
        if (allowJump && Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Debug.Log("player jumped");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        Vector3 velocity = inputDirection * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    public void SetZMovementEnabled(bool enabled)
    {
        allowZMovement = enabled;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 简单的地面检测（可以替换为更精准的射线检测）
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}