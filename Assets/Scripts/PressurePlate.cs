using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 压力板，根据承重触发事件
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [Header("压力板设置")]
    [SerializeField] private float activationWeight = 10f;    // 激活所需重量
    [SerializeField] private bool requiresExactWeight = false; // 是否需要精确重量
    [SerializeField] private float weightTolerance = 1f;      // 重量容差
    [SerializeField] private float plateDepression = 0.1f;    // 按下时的下降量
    
    [Header("视觉反馈")]
    [SerializeField] private Transform plateTransform;        // 压力板视觉部分
    [SerializeField] private Material activeMaterial;         // 激活状态的材质
    [SerializeField] private Material inactiveMaterial;       // 未激活状态的材质
    [SerializeField] private Color activeColor = Color.green; // 激活状态的颜色
    [SerializeField] private Color inactiveColor = Color.red; // 未激活状态的颜色
    
    [Header("音效")]
    [SerializeField] private AudioClip activationSound;       // 激活音效
    [SerializeField] private AudioClip deactivationSound;     // 取消激活音效
    
    [Header("事件")]
    public UnityEvent OnActivated;   // 激活时触发的事件
    public UnityEvent OnDeactivated; // 取消激活时触发的事件
    
    // 组件引用
    private Renderer plateRenderer;
    private AudioSource audioSource;
    private Vector3 originalPosition;
    private Vector3 depressedPosition;
    
    // 状态
    private bool isActive = false;
    private Dictionary<Rigidbody, float> objectsOnPlate = new Dictionary<Rigidbody, float>();
    private float currentWeight = 0f;
    
    // 公开属性
    public bool IsActive { get { return isActive; } }
    public float CurrentWeight { get { return currentWeight; } }

    private void Awake()
    {
        // 获取组件引用
        if (plateTransform == null)
            plateTransform = transform;
            
        plateRenderer = plateTransform.GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        
        // 如果没有音频源，但有声音，添加一个
        if (audioSource == null && (activationSound != null || deactivationSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 保存原始位置
        originalPosition = plateTransform.localPosition;
        depressedPosition = originalPosition - new Vector3(0, plateDepression, 0);
        
        // 初始视觉设置
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
    /// 更新当前重量并检查激活条件
    /// </summary>
    private void UpdateWeight()
    {
        // 计算当前重量
        currentWeight = 0f;
        foreach (var weight in objectsOnPlate.Values)
        {
            currentWeight += weight;
        }
        
        // 检查是否应该激活
        bool shouldBeActive;
        if (requiresExactWeight)
        {
            // 精确重量模式，在容差范围内
            shouldBeActive = Mathf.Abs(currentWeight - activationWeight) <= weightTolerance;
        }
        else
        {
            // 最低重量模式
            shouldBeActive = currentWeight >= activationWeight;
        }
        
        // 如果状态需要改变
        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;
            
            // 更新视觉
            UpdateVisuals();
            
            // 播放音效
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
            
            // 触发事件
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
    /// 更新压力板的视觉效果
    /// </summary>
    private void UpdateVisuals()
    {
        // 更新位置
        plateTransform.localPosition = isActive ? depressedPosition : originalPosition;
        
        // 更新材质/颜色
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
                // 如果材质未设置，至少更改颜色
                plateRenderer.material.color = targetColor;
            }
        }
    }
    
    /// <summary>
    /// 手动激活压力板（调试用）
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
    /// 手动取消激活压力板（调试用）
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
