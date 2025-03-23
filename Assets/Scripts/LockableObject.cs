using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LockableObject : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        LockTransform(); // 默认锁定
    }

    public void LockTransform()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void UnlockTransform()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}