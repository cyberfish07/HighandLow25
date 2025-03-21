using UnityEngine;

/// <summary>
/// �������ӵ��������Ժ�״̬
/// </summary>
public class Box : MonoBehaviour
{
    // ���ӵ�����״̬����
    [Header("��������")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material antiGravityMaterial;


    [Header("��������")]
    [SerializeField] private float normalGravityScale = 1.0f;
    [SerializeField] private float antiGravityScale = -1.0f;  // ��ֵ��ʾ������
    [SerializeField] private float transitionSpeed = 2.0f;   // ״̬�л��ٶ�

    // �������
    private Rigidbody rb;
    private Renderer boxRenderer;

    // ��ǰ״̬
    public enum BoxState { Normal, AntiGravity }
    [SerializeField] private BoxState currentState = BoxState.Normal;

    // ��������
    public BoxState CurrentState { get { return currentState; } }

    private void Awake()
    {
        // ��ȡ�������
        rb = GetComponent<Rigidbody>();
        boxRenderer = GetComponent<Renderer>();

        // ȷ��������Rigidbody
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // ��ʼ����
        UpdateVisuals();
        UpdatePhysics();
    }

    /// <summary>
    /// �л�����״̬
    /// </summary>
    public void ToggleState()
    {
        // �л�״̬
        currentState = (currentState == BoxState.Normal) ? BoxState.AntiGravity : BoxState.Normal;

        // �����Ӿ�������Ч��
        UpdateVisuals();
        UpdatePhysics();
    }

    /// <summary>
    /// �������ӵ�ָ��״̬
    /// </summary>
    public void SetState(BoxState newState)
    {
        // ���״̬�Ѿ���ͬ�������κβ���
        if (currentState == newState) return;

        // ������״̬
        currentState = newState;

        // �����Ӿ�������Ч��
        UpdateVisuals();
        UpdatePhysics();
    }

    /// <summary>
    /// �������ӵ��Ӿ�Ч��
    /// </summary>
    private void UpdateVisuals()
    {
        // ����״̬���²���
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
    /// �������ӵ���������
    /// </summary>
    private void UpdatePhysics()
    {
        if (rb != null)
        {
            // ��������
            Vector3 targetGravity = Physics.gravity * (currentState == BoxState.Normal ? normalGravityScale : antiGravityScale);

            // ���ü��ٶ�
            rb.linearVelocity = Vector3.zero;

            // ֱ��Ӧ������
            rb.useGravity = false;  // ����Ĭ������
            rb.AddForce(targetGravity, ForceMode.Acceleration);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null && !rb.useGravity)
        {
            // ����Ӧ�ö��Ƶ���������
            Vector3 customGravity = Physics.gravity * (currentState == BoxState.Normal ? normalGravityScale : antiGravityScale);
            rb.AddForce(customGravity, ForceMode.Acceleration);
        }
    }
}
