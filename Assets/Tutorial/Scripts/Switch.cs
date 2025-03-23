using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 开关基类，可以触发不同的事件
/// </summary>
public class Switch : MonoBehaviour
{
    [Header("开关设置")]
    [SerializeField] private bool oneTimeActivation = false;  // 是否只能触发一次
    [SerializeField] private float activationDelay = 0.1f;    // 激活延迟
    [SerializeField] private bool requiresPlayerToActivate = false; // 是否需要玩家来激活
    [SerializeField] private string playerTag = "Player";     // 玩家标签
    
    [Header("视觉反馈")]
    [SerializeField] private Material activeMaterial;        // 激活状态的材质
    [SerializeField] private Material inactiveMaterial;      // 未激活状态的材质
    [SerializeField] private Color activeColor = Color.green; // 激活状态的颜色
    [SerializeField] private Color inactiveColor = Color.red; // 未激活状态的颜色
    
    [Header("音效")]
    [SerializeField] private AudioClip activationSound;      // 激活音效
    
    [Header("事件")]
    public UnityEvent OnActivated;   // 激活时触发的事件
    public UnityEvent OnDeactivated; // 取消激活时触发的事件
    
    // 组件引用
    private Renderer switchRenderer;
    private AudioSource audioSource;
    
    // 状态
    private bool isActive = false;
    private bool hasBeenActivated = false;
    
    // 公开属性
    public bool IsActive { get { return isActive; } }

    private void Awake()
    {
        // 获取组件引用
        switchRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        
        // 如果没有音频源，但有声音，添加一个
        if (audioSource == null && activationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 初始视觉设置
        UpdateVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否需要玩家激活，以及触发对象是否合适
        if (requiresPlayerToActivate && !other.CompareTag(playerTag))
            return;
            
        // 如果是单次激活并且已经被激活过，则不再触发
        if (oneTimeActivation && hasBeenActivated)
            return;
            
        // 如果开关当前不是激活状态，则激活它
        if (!isActive)
        {
            StartCoroutine(ActivateWithDelay());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 如果不是单次激活，并且是相关对象离开，可以取消激活
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
    /// 带延迟激活开关
    /// </summary>
    private IEnumerator ActivateWithDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        
        isActive = true;
        hasBeenActivated = true;
        
        // 更新视觉效果
        UpdateVisuals();
        
        // 播放音效
        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
        
        // 触发事件
        OnActivated.Invoke();
    }

    /// <summary>
    /// 手动激活开关
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
    /// 手动重置开关
    /// </summary>
    public void Reset()
    {
        isActive = false;
        hasBeenActivated = false;
        UpdateVisuals();
    }

    /// <summary>
    /// 更新开关的视觉效果
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
                // 如果材质未设置，至少更改颜色
                switchRenderer.material.color = targetColor;
            }
        }
    }
}
