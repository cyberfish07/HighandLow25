using UnityEngine;

/// <summary>
/// �����л����أ�ר�����ڸı����ӵ�����״̬
/// </summary>
public class GravitySwitch : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private Switch switchComponent;    // ���ÿ������
    [SerializeField] private bool setAntiGravity = true; // true=����Ϊ������, false=����Ϊ��������
    [SerializeField] private float effectRadius = 2.0f; // Ӱ��뾶��Ϊ0��ֻӰ�촥��������
    [SerializeField] private LayerMask boxLayer;        // �������ڵĲ�

    [Header("�Ӿ�Ч��")]
    [SerializeField] private GameObject effectPrefab;   // �л�Ч��Ԥ����
    [SerializeField] private float effectDuration = 1.0f; // Ч������ʱ��

    private void OnEnable()
    {
        // ����п�������������伤���¼�
        if (switchComponent != null)
        {
            switchComponent.OnActivated.AddListener(ChangeGravity);
        }
    }

    private void OnDisable()
    {
        // ȡ�������¼�
        if (switchComponent != null)
        {
            switchComponent.OnActivated.RemoveListener(ChangeGravity);
        }
    }

    /// <summary>
    /// �ı���Χ���ӵ�����״̬
    /// </summary>
    public void ChangeGravity()
    {
        // ȷ��Ŀ������״̬
        Box.BoxState targetState = setAntiGravity ? Box.BoxState.AntiGravity : Box.BoxState.Normal;

        // ���Ч���뾶����0��Ѱ�ҷ�Χ�ڵ���������
        if (effectRadius > 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius, boxLayer);
            foreach (var collider in colliders)
            {
                Box box = collider.GetComponent<Box>();
                if (box != null)
                {
                    // ��������״̬
                    box.SetState(targetState);
                }
            }
        }

        // �����Ӿ�����Ч
        ShowEffect(transform.position);
    }

    /// <summary>
    /// ��ײ��⣬�����ӽӴ�����ʱ����״̬�ı�
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // ��������˿���������ɿ������������߼�
        if (switchComponent != null)
            return;

        // ֱ�Ӽ����ײ���Ƿ���Box���
        Box box = other.GetComponent<Box>();
        if (box != null)
        {
            // ����״̬
            box.SetState(setAntiGravity ? Box.BoxState.AntiGravity : Box.BoxState.Normal);

            // ��ʾ�Ӿ�Ч��
            ShowEffect(box.transform.position);
        }
    }

    /// <summary>
    /// ��ʾ״̬�л��Ӿ�Ч��
    /// </summary>
    private void ShowEffect(Vector3 position)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }
    }
}
