using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public bool allowJump = true;

    [Header("Input Keys")]
    public KeyCode jumpKey = KeyCode.Space;
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";

    private bool allowZMovement = false;
    private bool isGrounded = true;

    private Rigidbody rb;
    private Vector3 inputDirection;

    [Header("References")]
    public CameraSwitcher cameraSwitcher;
    public Animator animator; // ✅ 动画器引用

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 自动获取 Animator（可选）
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // 获取当前是否允许 Z 轴移动（基于摄像机）
        if (cameraSwitcher != null)
        {
            allowZMovement = cameraSwitcher.isThreeD;
        }

        float x = Input.GetAxisRaw(horizontalAxis);
        float z = allowZMovement ? Input.GetAxisRaw(verticalAxis) : 0f;

        inputDirection = new Vector3(x, 0f, z).normalized;

        // ✅ 更新动画参数
        float speed = inputDirection.magnitude;
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }

        // 跳跃
        if (allowJump && Input.GetKeyDown(jumpKey) && isGrounded)
        {
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
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}
