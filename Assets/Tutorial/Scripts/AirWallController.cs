using UnityEngine;

/// <summary>
/// 控制空气墙在不同视角下的显示状态
/// 空气墙在2D视角下显示，3D视角下隐藏
/// </summary>
public class AirWallController : MonoBehaviour
{
    [Header("相机切换器引用")]
    [SerializeField] private CameraSwitcher cameraSwitcher;

    [Header("调试选项")]
    [SerializeField] private bool showDebugLogs = false;

    // 记录上一帧的视角状态
    private bool lastIsThreeD = false;
    private bool isInitialized = false;

    private void Start()
    {
        InitializeController();
    }

    private void OnEnable()
    {
        // 当物体启用时确保状态正确
        if (isInitialized)
        {
            UpdateVisibility();
        }
    }

    private void InitializeController()
    {
        // 如果没有指定相机切换器，尝试在场景中查找
        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
            if (cameraSwitcher == null)
            {
                Debug.LogError($"{gameObject.name}: 无法找到CameraSwitcher组件！");
                return;
            }
        }

        // 初始化状态
        lastIsThreeD = cameraSwitcher.isThreeD;
        UpdateVisibility();
        isInitialized = true;

        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} 初始化 - 3D模式: {cameraSwitcher.isThreeD}, 墙状态: {gameObject.activeSelf}");
        }
    }

    private void Update()
    {
        // 如果未初始化，尝试初始化
        if (!isInitialized)
        {
            InitializeController();
            return;
        }

        if (cameraSwitcher != null)
        {
            // 检查视角状态是否变化
            if (lastIsThreeD != cameraSwitcher.isThreeD)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"{gameObject.name}: 检测到视角切换 - 从 {(lastIsThreeD ? "3D" : "2D")} 到 {(cameraSwitcher.isThreeD ? "3D" : "2D")}");
                }

                lastIsThreeD = cameraSwitcher.isThreeD;
                UpdateVisibility();
            }
        }
        else
        {
            // 如果cameraSwitcher变为null，尝试重新获取
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        }
    }

    // 更新空气墙可见性
    private void UpdateVisibility()
    {
        if (cameraSwitcher == null) return;

        // 2D视角下显示，3D视角下隐藏
        bool shouldBeActive = !cameraSwitcher.isThreeD;

        // 设置激活状态
        gameObject.SetActive(shouldBeActive);

        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: 空气墙状态更新 - 当前视角: {(cameraSwitcher.isThreeD ? "3D" : "2D")}, 墙状态: {(shouldBeActive ? "显示" : "隐藏")}");
        }
    }

    // 提供外部调用方法强制更新
    public void ForceUpdateVisibility()
    {
        UpdateVisibility();
    }
}