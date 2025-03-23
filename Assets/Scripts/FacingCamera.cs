using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    private Transform[] children;

    void Start()
    {
        children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
    }

    void LateUpdate() // 用 LateUpdate 可以避免和相机移动的更新顺序问题
    {
        if (Camera.main == null) return;

        Quaternion targetRotation = Camera.main.transform.rotation;

        for (int i = 0; i < children.Length; i++)
        {
            children[i].rotation = targetRotation;
        }
    }
}
