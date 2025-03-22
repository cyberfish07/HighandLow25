using UnityEngine;

/// <summary>
/// 控制箱子的物理属性和状态
/// </summary>
public class Box : MonoBehaviour
{
    // 箱子的两种状态材质
    [Header("材质设置")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material antiGravityMaterial;


    [Header("物理设置")]
    [SerializeField] private float normalGravityScale = 1.0f;
    [SerializeField] private float antiGravityScale = -1.0f;  // 负值表示反重力
    [SerializeField] private float transitionSpeed = 2.0f;   // 状态切换速度

    // 引用组件
    private Rigidbody rb;
    private Renderer boxRenderer;

    // 当前状态
    public enum BoxState { Normal, AntiGravity }
    [SerializeField] private BoxState currentState = BoxState.Normal;

    // 公开属性
    public BoxState CurrentState { get { return currentState; } }

    private void Awake()
    {
        // 获取组件引用
        rb = GetComponent<Rigidbody>();
        boxRenderer = GetComponent<Renderer>();

        // 确保箱子有Rigidbody
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // 初始设置
        UpdateVisuals();
        UpdatePhysics();
    }

    /// <summary>
    /// 切换箱子状态
    /// </summary>
    public void ToggleState()
    {
        // 切换状态
        currentState = (currentState == BoxState.Normal) ? BoxState.AntiGravity : BoxState.Normal;

        // 更新视觉和物理效果
        UpdateVisuals();
        UpdatePhysics();
    }

    /// <summary>
    /// 设置箱子到指定状态
    /// </summary>
    public void SetState(BoxState newState)
    {
        // 如果状态已经相同，不做任何操作
        if (currentState == newState) return;

        // 设置新状态
        currentState = newState;

        // 更新视觉和物理效果
        UpdateVisuals();
        UpdatePhysics();
    }

    /// <summary>
    /// 更新箱子的视觉效果
    /// </summary>
    private void UpdateVisuals()
    {
        // 根据状态更新材质
        if (boxRenderer != null)
        {
            Material targetMaterial = (currentState == BoxState.Normal) ? normalMaterial : antiGravityMaterial;


            if (targetMaterial != null)
            {
                boxRenderer.material = targetMaterial;
            }
        }
    }

    /// <summary>
    /// 更新箱子的物理属性
    /// </summary>
    private void UpdatePhysics()
    {
        if (rb != null)
        {
            // 设置重力
            Vector3 targetGravity = Physics.gravity * (currentState == BoxState.Normal ? normalGravityScale : antiGravityScale);

            // 设置加速度
            rb.linearVelocity = Vector3.zero;

            // 直接应用重力
            rb.useGravity = false;  // 禁用默认重力
            rb.AddForce(targetGravity, ForceMode.Acceleration);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null && !rb.useGravity)
        {
            // 持续应用定制的重力力量
            Vector3 customGravity = Physics.gravity * (currentState == BoxState.Normal ? normalGravityScale : antiGravityScale);
            rb.AddForce(customGravity, ForceMode.Acceleration);
        }
    }
}
