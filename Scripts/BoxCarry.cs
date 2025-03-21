using UnityEngine;

public class BoxCarry : MonoBehaviour
{
    public Transform carryPoint; // 你在玩家前方放一个空物体作为“抓取位置”
    public float detectionRadius = 1.5f;
    public LayerMask boxLayer;
    public KeyCode grabKey = KeyCode.E;

    private GameObject carriedBox = null;

    void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            if (carriedBox == null)
            {
                TryGrabBox();
            }
            else
            {
                DropBox();
            }
        }

        if (carriedBox != null)
        {
            carriedBox.transform.position = carryPoint.position;
        }
    }

    void TryGrabBox()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, boxLayer);
        if (hits.Length > 0)
        {
            carriedBox = hits[0].gameObject;

            var rb = carriedBox.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // 让它不受物理干扰
                rb.useGravity = false;
            }

            // 如果你有 BoxController 之类的，也可以做额外标记
        }
    }

    void DropBox()
    {
        if (carriedBox != null)
        {
            var rb = carriedBox.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            carriedBox = null;
        }
    }
}