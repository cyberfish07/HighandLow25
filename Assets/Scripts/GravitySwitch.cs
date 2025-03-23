using UnityEngine;

/// <summary>
/// 重力切换开关，专门用于改变箱子的重力状态
/// </summary>
public class GravitySwitch : MonoBehaviour
{
    [Header("开关设置")]
    [SerializeField] private Switch switchComponent;    // 引用开关组件
    [SerializeField] private bool setAntiGravity = true; // true=设置为反重力, false=设置为正常重力
    [SerializeField] private float effectRadius = 2.0f; // 影响半径，为0则只影响触碰的物体
    [SerializeField] private LayerMask boxLayer;        // 箱子所在的层
    
    [Header("视觉效果")]
    [SerializeField] private GameObject effectPrefab;   // 切换效果预制体
    [SerializeField] private float effectDuration = 1.0f; // 效果持续时间
    
    private void OnEnable()
    {
        // 如果有开关组件，订阅其激活事件
        if (switchComponent != null)
        {
            switchComponent.OnActivated.AddListener(ChangeGravity);
        }
    }
    
    private void OnDisable()
    {
        // 取消订阅事件
        if (switchComponent != null)
        {
            switchComponent.OnActivated.RemoveListener(ChangeGravity);
        }
    }
    
    /// <summary>
    /// 改变周围箱子的重力状态
    /// </summary>
    public void ChangeGravity()
    {
        // 确定目标箱子状态
        Box.BoxState targetState = setAntiGravity ? Box.BoxState.AntiGravity : Box.BoxState.Normal;
        
        // 如果效果半径大于0，寻找范围内的所有箱子
        if (effectRadius > 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius, boxLayer);
            foreach (var collider in colliders)
            {
                Box box = collider.GetComponent<Box>();
                if (box != null)
                {
                    // 设置箱子状态
                    box.SetState(targetState);
                    
                    // 显示视觉效果
                    ShowEffect(box.transform.position);
                }
            }
        }
        
        // 播放视觉和音效
        ShowEffect(transform.position);
    }
    
    /// <summary>
    /// 碰撞检测，当箱子接触开关时触发状态改变
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 如果设置了开关组件，由开关来处理触发逻辑
        if (switchComponent != null)
            return;
            
        // 直接检查碰撞体是否有Box组件
        Box box = other.GetComponent<Box>();
        if (box != null)
        {
            // 设置状态
            box.SetState(setAntiGravity ? Box.BoxState.AntiGravity : Box.BoxState.Normal);
            
            // 显示视觉效果
            ShowEffect(box.transform.position);
        }
    }
    
    /// <summary>
    /// 显示状态切换视觉效果
    /// </summary>
    private void ShowEffect(Vector3 position)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }
    }
    
    /// <summary>
    /// 可视化效果半径
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (effectRadius > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, effectRadius);
        }
    }
}
