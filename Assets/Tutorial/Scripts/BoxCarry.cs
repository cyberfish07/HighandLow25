using UnityEngine;

public class BoxCarry : MonoBehaviour
{
    public Transform carryPoint; //
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
                rb.isKinematic = true; // 
                rb.useGravity = false;
            }

            // BoxController
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