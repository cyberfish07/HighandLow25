using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ����ǽ������ - ���п������п���ǽ����ʾ/����
/// </summary>
public class AirWallManager : MonoBehaviour
{
    [Header("����л�������")]
    [SerializeField] private CameraSwitcher cameraSwitcher;

    [Header("����ǽ��ǩ����")]
    [SerializeField] private string airWall2DTag = "AirWall2D"; // ��2D�ӽ���ʾ�Ŀ���ǽ��ǩ

    [Header("����ѡ��")]
    [SerializeField] private bool showDebugLogs = false;

    // �洢�ҵ������п���ǽ
    private List<GameObject> airWalls2D = new List<GameObject>();
    private List<GameObject> airWalls3D = new List<GameObject>();

    // Э������
    private Coroutine monitorCoroutine;

    private void Awake()
    {
        // ��Awake�о������CameraSwitcher
        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        }
    }

    private void Start()
    {
        // �������п���ǽ������
        FindAllAirWalls();

        // ��ʼ������״̬
        StartMonitoring();
    }

    private void OnEnable()
    {
        // ����ű������ú��������ã����¿�ʼ���
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
        // ��������б�
        airWalls2D.Clear();
        airWalls3D.Clear();

        // ��������2D����ǽ
        GameObject[] walls2D = GameObject.FindGameObjectsWithTag(airWall2DTag);
        foreach (var wall in walls2D)
        {
            airWalls2D.Add(wall);
        }
    }

    /// <summary>
    /// ��ʼ������״̬�仯
    /// </summary>
    private void StartMonitoring()
    {
        if (monitorCoroutine != null)
        {
            StopCoroutine(monitorCoroutine);
        }

        monitorCoroutine = StartCoroutine(MonitorCameraState());

        // ��������һ�����п���ǽ״̬
        UpdateAllAirWalls();
    }

    /// <summary>
    /// ������״̬��Э��
    /// </summary>
    private IEnumerator MonitorCameraState()
    {
        // �ȴ�һ֡ȷ������������ѳ�ʼ��
        yield return null;

        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
            if (cameraSwitcher == null)
            {
                Debug.LogError("AirWallManager: �޷��ҵ�CameraSwitcher�����");
                yield break;
            }
        }

        bool lastThreeDState = cameraSwitcher.isThreeD;

        while (true)
        {
            // ȷ��CameraSwitcher��Ȼ��Ч
            if (cameraSwitcher == null)
            {
                cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
                if (cameraSwitcher == null)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
            }

            // ���״̬�仯
            if (lastThreeDState != cameraSwitcher.isThreeD)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"AirWallManager: ��⵽�ӽ��л� - �� {(lastThreeDState ? "3D" : "2D")} �� {(cameraSwitcher.isThreeD ? "3D" : "2D")}");
                }

                lastThreeDState = cameraSwitcher.isThreeD;

                // �ȴ�һ֡ȷ���������л�
                yield return null;

                // �������п���ǽ״̬
                UpdateAllAirWalls();
            }

            // ���ݵȴ��ټ��
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// �������п���ǽ����ʾ/����״̬
    /// </summary>
    private void UpdateAllAirWalls()
    {
        bool is3DMode = cameraSwitcher.isThreeD;

        // ����2D����ǽ (2Dģʽ����ʾ)
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
                        Debug.Log($"2D����ǽ '{wall.name}' ��{(shouldBeActive ? "��ʾ" : "����")}");
                    }
                }
            }
        }

        // ����3D����ǽ (3Dģʽ����ʾ)
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
                        Debug.Log($"3D����ǽ '{wall.name}' ��{(shouldBeActive ? "��ʾ" : "����")}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// ����ɨ�賡���еĿ���ǽ
    /// </summary>
    public void RescanAirWalls()
    {
        FindAllAirWalls();
        UpdateAllAirWalls();
    }

    /// <summary>
    /// ǿ�Ƹ������п���ǽ״̬
    /// </summary>
    public void ForceUpdateAllAirWalls()
    {
        UpdateAllAirWalls();
    }
}