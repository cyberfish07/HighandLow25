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
    private bool facingRight = true; // ✅ 朝向控制

    private Rigidbody rb;
    private Vector3 inputDirection;

    [Header("References")]
    public CameraSwitcher cameraSwitcher;
    public Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // 判断是否允许Z轴移动
        if (cameraSwitcher != null)
        {
            allowZMovement = cameraSwitcher.isThreeD;
        }

        // 获取输入
        float x = Input.GetAxisRaw(horizontalAxis);
        float z = allowZMovement ? Input.GetAxisRaw(verticalAxis) : 0f;

        inputDirection = new Vector3(x, 0f, z).normalized;

        // ✅ 设置动画参数（Speed）
        float speed = inputDirection.magnitude;
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }

        // ✅ 控制角色旋转方向
        if (inputDirection.x < 0 && facingRight)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            facingRight = false;
        }
        else if (inputDirection.x > 0 && !facingRight)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            facingRight = true;
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
        // 粗略的地面检测
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}
