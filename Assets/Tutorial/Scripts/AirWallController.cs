using UnityEngine;

/// <summary>
/// ���ƿ���ǽ�ڲ�ͬ�ӽ��µ���ʾ״̬
/// ����ǽ��2D�ӽ�����ʾ��3D�ӽ�������
/// </summary>
public class AirWallController : MonoBehaviour
{
    [Header("����л�������")]
    [SerializeField] private CameraSwitcher cameraSwitcher;

    [Header("����ѡ��")]
    [SerializeField] private bool showDebugLogs = false;

    // ��¼��һ֡���ӽ�״̬
    private bool lastIsThreeD = false;
    private bool isInitialized = false;

    private void Start()
    {
        InitializeController();
    }

    private void OnEnable()
    {
        // ����������ʱȷ��״̬��ȷ
        if (isInitialized)
        {
            UpdateVisibility();
        }
    }

    private void InitializeController()
    {
        // ���û��ָ������л����������ڳ����в���
        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
            if (cameraSwitcher == null)
            {
                Debug.LogError($"{gameObject.name}: �޷��ҵ�CameraSwitcher�����");
                return;
            }
        }

        // ��ʼ��״̬
        lastIsThreeD = cameraSwitcher.isThreeD;
        UpdateVisibility();
        isInitialized = true;

        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} ��ʼ�� - 3Dģʽ: {cameraSwitcher.isThreeD}, ǽ״̬: {gameObject.activeSelf}");
        }
    }

    private void Update()
    {
        // ���δ��ʼ�������Գ�ʼ��
        if (!isInitialized)
        {
            InitializeController();
            return;
        }

        if (cameraSwitcher != null)
        {
            // ����ӽ�״̬�Ƿ�仯
            if (lastIsThreeD != cameraSwitcher.isThreeD)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"{gameObject.name}: ��⵽�ӽ��л� - �� {(lastIsThreeD ? "3D" : "2D")} �� {(cameraSwitcher.isThreeD ? "3D" : "2D")}");
                }

                lastIsThreeD = cameraSwitcher.isThreeD;
                UpdateVisibility();
            }
        }
        else
        {
            // ���cameraSwitcher��Ϊnull���������»�ȡ
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        }
    }

    // ���¿���ǽ�ɼ���
    private void UpdateVisibility()
    {
        if (cameraSwitcher == null) return;

        // 2D�ӽ�����ʾ��3D�ӽ�������
        bool shouldBeActive = !cameraSwitcher.isThreeD;

        // ���ü���״̬
        gameObject.SetActive(shouldBeActive);

        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: ����ǽ״̬���� - ��ǰ�ӽ�: {(cameraSwitcher.isThreeD ? "3D" : "2D")}, ǽ״̬: {(shouldBeActive ? "��ʾ" : "����")}");
        }
    }

    // �ṩ�ⲿ���÷���ǿ�Ƹ���
    public void ForceUpdateVisibility()
    {
        UpdateVisibility();
    }
}