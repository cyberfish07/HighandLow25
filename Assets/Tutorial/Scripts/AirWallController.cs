using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 空气墙管理器 - 集中控制所有空气墙的显示/隐藏
/// </summary>
public class AirWallManager : MonoBehaviour
{
    [Header("相机切换器引用")]
    [SerializeField] private CameraSwitcher cameraSwitcher;

    [Header("空气墙标签设置")]
    [SerializeField] private string airWall2DTag = "AirWall2D"; // 在2D视角显示的空气墙标签

    [Header("调试选项")]
    [SerializeField] private bool showDebugLogs = false;

    // 存储找到的所有空气墙
    private List<GameObject> airWalls2D = new List<GameObject>();
    private List<GameObject> airWalls3D = new List<GameObject>();

    // 协程引用
    private Coroutine monitorCoroutine;

    private void Awake()
    {
        // 在Awake中尽早查找CameraSwitcher
        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        }
    }

    private void Start()
    {
        // 查找所有空气墙并分类
        FindAllAirWalls();

        // 开始监控相机状态
        StartMonitoring();
    }

    private void OnEnable()
    {
        // 如果脚本被禁用后重新启用，重新开始监控
        if (monitorCoroutine == null)
        {
            StartMonitoring();
        }
    }

    private void OnDisable()
    {
        if (monitorCoroutine != null)
        {
            StopCoroutine(monitorCoroutine);
            monitorCoroutine = null;
        }
    }

    private void FindAllAirWalls()
    {
        // 清空现有列表
        airWalls2D.Clear();
        airWalls3D.Clear();

        // 查找所有2D空气墙
        GameObject[] walls2D = GameObject.FindGameObjectsWithTag(airWall2DTag);
        foreach (var wall in walls2D)
        {
            airWalls2D.Add(wall);
        }
    }

    /// <summary>
    /// 开始监控相机状态变化
    /// </summary>
    private void StartMonitoring()
    {
        if (monitorCoroutine != null)
        {
            StopCoroutine(monitorCoroutine);
        }

        monitorCoroutine = StartCoroutine(MonitorCameraState());

        // 立即更新一次所有空气墙状态
        UpdateAllAirWalls();
    }

    /// <summary>
    /// 监控相机状态的协程
    /// </summary>
    private IEnumerator MonitorCameraState()
    {
        // 等待一帧确保所有组件都已初始化
        yield return null;

        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
            if (cameraSwitcher == null)
            {
                Debug.LogError("AirWallManager: 无法找到CameraSwitcher组件！");
                yield break;
            }
        }

        bool lastThreeDState = cameraSwitcher.isThreeD;

        while (true)
        {
            // 确保CameraSwitcher仍然有效
            if (cameraSwitcher == null)
            {
                cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
                if (cameraSwitcher == null)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
            }

            // 检测状态变化
            if (lastThreeDState != cameraSwitcher.isThreeD)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"AirWallManager: 检测到视角切换 - 从 {(lastThreeDState ? "3D" : "2D")} 到 {(cameraSwitcher.isThreeD ? "3D" : "2D")}");
                }

                lastThreeDState = cameraSwitcher.isThreeD;

                // 等待一帧确保相机完成切换
                yield return null;

                // 更新所有空气墙状态
                UpdateAllAirWalls();
            }

            // 短暂等待再检查
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// 更新所有空气墙的显示/隐藏状态
    /// </summary>
    private void UpdateAllAirWalls()
    {
        bool is3DMode = cameraSwitcher.isThreeD;

        // 更新2D空气墙 (2D模式下显示)
        foreach (var wall in airWalls2D)
        {
            if (wall != null)
            {
                bool shouldBeActive = !is3DMode;
                if (wall.activeSelf != shouldBeActive)
                {
                    wall.SetActive(shouldBeActive);

                    if (showDebugLogs)
                    {
                        Debug.Log($"2D空气墙 '{wall.name}' 已{(shouldBeActive ? "显示" : "隐藏")}");
                    }
                }
            }
        }

        // 更新3D空气墙 (3D模式下显示)
        foreach (var wall in airWalls3D)
        {
            if (wall != null)
            {
                bool shouldBeActive = is3DMode;
                if (wall.activeSelf != shouldBeActive)
                {
                    wall.SetActive(shouldBeActive);

                    if (showDebugLogs)
                    {
                        Debug.Log($"3D空气墙 '{wall.name}' 已{(shouldBeActive ? "显示" : "隐藏")}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 重新扫描场景中的空气墙
    /// </summary>
    public void RescanAirWalls()
    {
        FindAllAirWalls();
        UpdateAllAirWalls();
    }

    /// <summary>
    /// 强制更新所有空气墙状态
    /// </summary>
    public void ForceUpdateAllAirWalls()
    {
        UpdateAllAirWalls();
    }
}