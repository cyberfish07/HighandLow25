using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ѹ���壬���ݳ��ش����¼�
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [Header("ѹ��������")]
    [SerializeField] private float activationWeight = 10f;    // ������������
    [SerializeField] private bool requiresExactWeight = false; // �Ƿ���Ҫ��ȷ����
    [SerializeField] private float weightTolerance = 1f;      // �����ݲ�
    [SerializeField] private float plateDepression = 0.1f;    // ����ʱ���½���

    [Header("�Ӿ�����")]
    [SerializeField] private Transform plateTransform;        // ѹ�����Ӿ�����
    [SerializeField] private Material activeMaterial;         // ����״̬�Ĳ���
    [SerializeField] private Material inactiveMaterial;       // δ����״̬�Ĳ���
    [SerializeField] private Color activeColor = Color.green; // ����״̬����ɫ
    [SerializeField] private Color inactiveColor = Color.red; // δ����״̬����ɫ

    [Header("��Ч")]
    [SerializeField] private AudioClip activationSound;       // ������Ч
    [SerializeField] private AudioClip deactivationSound;     // ȡ��������Ч

    [Header("�¼�")]
    public UnityEvent OnActivated;   // ����ʱ�������¼�
    public UnityEvent OnDeactivated; // ȡ������ʱ�������¼�

    // �������
    private Renderer plateRenderer;
    private AudioSource audioSource;
    private Vector3 originalPosition;
    private Vector3 depressedPosition;

    // ״̬
    private bool isActive = false;
    private Dictionary<Rigidbody, float> objectsOnPlate = new Dictionary<Rigidbody, float>();
    private float currentWeight = 0f;

    // ��������
    public bool IsActive { get { return isActive; } }
    public float CurrentWeight { get { return currentWeight; } }

    private void Awake()
    {
        // ��ȡ�������
        if (plateTransform == null)
            plateTransform = transform;

        plateRenderer = plateTransform.GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        // ���û����ƵԴ���������������һ��
        if (audioSource == null && (activationSound != null || deactivationSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ����ԭʼλ��
        originalPosition = plateTransform.localPosition;
        depressedPosition = originalPosition - new Vector3(0, plateDepression, 0);

        // ��ʼ�Ӿ�����
        UpdateVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !objectsOnPlate.ContainsKey(rb))
        {
            objectsOnPlate.Add(rb, rb.mass);
            UpdateWeight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && objectsOnPlate.ContainsKey(rb))
        {
            objectsOnPlate.Remove(rb);
            UpdateWeight();
        }
    }

    /// <summary>
    /// ���µ�ǰ��������鼤������
    /// </summary>
    private void UpdateWeight()
    {
        // ���㵱ǰ����
        currentWeight = 0f;
        foreach (var weight in objectsOnPlate.Values)
        {
            currentWeight += weight;
        }

        // ����Ƿ�Ӧ�ü���
        bool shouldBeActive;
        if (requiresExactWeight)
        {
            // ��ȷ����ģʽ�����ݲΧ��
            shouldBeActive = Mathf.Abs(currentWeight - activationWeight) <= weightTolerance;
        }
        else
        {
            // �������ģʽ
            shouldBeActive = currentWeight >= activationWeight;
        }

        // ���״̬��Ҫ�ı�
        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;

            // �����Ӿ�
            UpdateVisuals();

            // ������Ч
            if (audioSource != null)
            {
                if (isActive && activationSound != null)
                {
                    audioSource.PlayOneShot(activationSound);
                }
                else if (!isActive && deactivationSound != null)
                {
                    audioSource.PlayOneShot(deactivationSound);
                }
            }

            // �����¼�
            if (isActive)
            {
                OnActivated.Invoke();
            }
            else
            {
                OnDeactivated.Invoke();
            }
        }
    }

    /// <summary>
    /// ����ѹ������Ӿ�Ч��
    /// </summary>
    private void UpdateVisuals()
    {
        // ����λ��
        plateTransform.localPosition = isActive ? depressedPosition : originalPosition;

        // ���²���/��ɫ
        if (plateRenderer != null)
        {
            Material targetMaterial = isActive ? activeMaterial : inactiveMaterial;
            Color targetColor = isActive ? activeColor : inactiveColor;

            if (targetMaterial != null)
            {
                plateRenderer.material = targetMaterial;
            }
            else
            {
                // �������δ���ã����ٸ�����ɫ
                plateRenderer.material.color = targetColor;
            }
        }
    }

    /// <summary>
    /// �ֶ�����ѹ���壨�����ã�
    /// </summary>
    public void ForceActivate()
    {
        if (!isActive)
        {
            isActive = true;
            UpdateVisuals();
            OnActivated.Invoke();
        }
    }

    /// <summary>
    /// �ֶ�ȡ������ѹ���壨�����ã�
    /// </summary>
    public void ForceDeactivate()
    {
        if (isActive)
        {
            isActive = false;
            UpdateVisuals();
            OnDeactivated.Invoke();
        }
    }
}

