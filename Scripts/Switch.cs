using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ���ػ��࣬���Դ�����ͬ���¼�
/// </summary>
public class Switch : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private bool oneTimeActivation = false;  // �Ƿ�ֻ�ܴ���һ��
    [SerializeField] private float activationDelay = 0.1f;    // �����ӳ�
    [SerializeField] private bool requiresPlayerToActivate = false; // �Ƿ���Ҫ���������
    [SerializeField] private string playerTag = "Player";     // ��ұ�ǩ

    [Header("�Ӿ�����")]
    [SerializeField] private Material activeMaterial;        // ����״̬�Ĳ���
    [SerializeField] private Material inactiveMaterial;      // δ����״̬�Ĳ���
    [SerializeField] private Color activeColor = Color.green; // ����״̬����ɫ
    [SerializeField] private Color inactiveColor = Color.red; // δ����״̬����ɫ

    [Header("��Ч")]
    [SerializeField] private AudioClip activationSound;      // ������Ч

    [Header("�¼�")]
    public UnityEvent OnActivated;   // ����ʱ�������¼�
    public UnityEvent OnDeactivated; // ȡ������ʱ�������¼�

    // �������
    private Renderer switchRenderer;
    private AudioSource audioSource;

    // ״̬
    private bool isActive = false;
    private bool hasBeenActivated = false;

    // ��������
    public bool IsActive { get { return isActive; } }

    private void Awake()
    {
        // ��ȡ�������
        switchRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        // ���û����ƵԴ���������������һ��
        if (audioSource == null && activationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ��ʼ�Ӿ�����
        UpdateVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����Ƿ���Ҫ��Ҽ���Լ����������Ƿ����
        if (requiresPlayerToActivate && !other.CompareTag(playerTag))
            return;

        // ����ǵ��μ�����Ѿ�������������ٴ���
        if (oneTimeActivation && hasBeenActivated)
            return;

        // ������ص�ǰ���Ǽ���״̬���򼤻���
        if (!isActive)
        {
            StartCoroutine(ActivateWithDelay());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ������ǵ��μ����������ض����뿪������ȡ������
        if (!oneTimeActivation)
        {
            if (requiresPlayerToActivate && !other.CompareTag(playerTag))
                return;

            if (isActive)
            {
                isActive = false;
                UpdateVisuals();
                OnDeactivated.Invoke();
            }
        }
    }

    /// <summary>
    /// ���ӳټ����
    /// </summary>
    private IEnumerator ActivateWithDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        isActive = true;
        hasBeenActivated = true;

        // �����Ӿ�Ч��
        UpdateVisuals();

        // ������Ч
        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }

        // �����¼�
        OnActivated.Invoke();
    }

    /// <summary>
    /// �ֶ������
    /// </summary>
    public void Activate()
    {
        if (oneTimeActivation && hasBeenActivated)
            return;

        if (!isActive)
        {
            StartCoroutine(ActivateWithDelay());
        }
    }

    /// <summary>
    /// �ֶ����ÿ���
    /// </summary>
    public void Reset()
    {
        isActive = false;
        hasBeenActivated = false;
        UpdateVisuals();
    }

    /// <summary>
    /// ���¿��ص��Ӿ�Ч��
    /// </summary>
    private void UpdateVisuals()
    {
        if (switchRenderer != null)
        {
            Material targetMaterial = isActive ? activeMaterial : inactiveMaterial;
            Color targetColor = isActive ? activeColor : inactiveColor;

            if (targetMaterial != null)
            {
                switchRenderer.material = targetMaterial;
            }
            else
            {
                // �������δ���ã����ٸ�����ɫ
                switchRenderer.material.color = targetColor;
            }
        }
    }
}

