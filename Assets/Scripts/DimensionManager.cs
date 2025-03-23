using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理2D和3D维度的切换
/// </summary>
public class DimensionManager : MonoBehaviour
{
    public enum Dimension { TwoD, ThreeD }
    
    [Header("维度设置")]
    [SerializeField] private Dimension currentDimension = Dimension.TwoD;
    [SerializeField] private KeyCode dimensionToggleKey = KeyCode.Tab;
    
    [Header("相机设置")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 twoDCameraPosition = new Vector3(0, 0, -10);
    [SerializeField] private Vector3 threeDCameraPosition = new Vector3(10, 5, -10);
    [SerializeField] private Vector3 twoDCameraRotation = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 threeDCameraRotation = new Vector3(30, -45, 0);
    [SerializeField] private float cameraTransitionSpeed = 5.0f;
    
    [Header("游戏对象设置")]
    [SerializeField] private List<GameObject> onlyVisibleIn2D = new List<GameObject>(); // 只在2D可见的物体
    [SerializeField] private List<GameObject> onlyVisibleIn3D = new List<GameObject>(); // 只在3D可见的物体
    
    // 内部状态
    private bool isTransitioning = false;
    
    // 公开属性
    public Dimension CurrentDimension { get { return currentDimension; } }
    public bool IsTransitioning { get { return isTransitioning; } }
    
    // 单例模式
    private static DimensionManager _instance;
    public static DimensionManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<DimensionManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 确保有主摄像机引用
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // 初始化维度
        SetInitialDimension();
    }

    private void Update()
    {
        // 检测切换维度的输入
        if (Input.GetKeyDown(dimensionToggleKey) && !isTransitioning)
        {
            ToggleDimension();
        }
    }

    /// <summary>
    /// 设置初始维度状态
    /// </summary>
    private void SetInitialDimension()
    {
        // 立即设置相机位置和旋转
        if (mainCamera != null)
        {
            if (currentDimension == Dimension.TwoD)
            {
                mainCamera.transform.position = twoDCameraPosition;
                mainCamera.transform.eulerAngles = twoDCameraRotation;
            }
            else
            {
                mainCamera.transform.position = threeDCameraPosition;
                mainCamera.transform.eulerAngles = threeDCameraRotation;
            }
        }
        
        // 设置物体可见性
        UpdateObjectVisibility();
    }

    /// <summary>
    /// 切换维度
    /// </summary>
    public void ToggleDimension()
    {
        currentDimension = (currentDimension == Dimension.TwoD) ? Dimension.ThreeD : Dimension.TwoD;
        StartTransition();
    }

    /// <summary>
    /// 直接设置到指定维度
    /// </summary>
    public void SetDimension(Dimension dimension)
    {
        if (currentDimension != dimension)
        {
            currentDimension = dimension;
            StartTransition();
        }
    }

    /// <summary>
    /// 开始维度转换过程
    /// </summary>
    private void StartTransition()
    {
        if (!isTransitioning && mainCamera != null)
        {
            isTransitioning = true;
            
            // 立即更新物体可见性
            UpdateObjectVisibility();
            
            // 开始相机过渡协程
            StartCoroutine(TransitionCamera());
        }
    }

    /// <summary>
    /// 相机平滑过渡协程
    /// </summary>
    private System.Collections.IEnumerator TransitionCamera()
    {
        Vector3 targetPosition = (currentDimension == Dimension.TwoD) ? twoDCameraPosition : threeDCameraPosition;
        Vector3 targetRotation = (currentDimension == Dimension.TwoD) ? twoDCameraRotation : threeDCameraRotation;
        
        // 继续过渡直到达到目标位置
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f || 
               Quaternion.Angle(mainCamera.transform.rotation, Quaternion.Euler(targetRotation)) > 0.01f)
        {
            // 平滑移动位置
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position, 
                targetPosition, 
                cameraTransitionSpeed * Time.deltaTime
            );
            
            // 平滑旋转
            mainCamera.transform.rotation = Quaternion.Lerp(
                mainCamera.transform.rotation, 
                Quaternion.Euler(targetRotation), 
                cameraTransitionSpeed * Time.deltaTime
            );
            
            yield return null;
        }
        
        // 确保完全达到目标
        mainCamera.transform.position = targetPosition;
        mainCamera.transform.eulerAngles = targetRotation;
        
        // 转换完成
        isTransitioning = false;
    }

    /// <summary>
    /// 更新物体可见性
    /// </summary>
    private void UpdateObjectVisibility()
    {
        // 更新2D专属物体
        foreach (var obj in onlyVisibleIn2D)
        {
            if (obj != null)
            {
                obj.SetActive(currentDimension == Dimension.TwoD);
            }
        }
        
        // 更新3D专属物体
        foreach (var obj in onlyVisibleIn3D)
        {
            if (obj != null)
            {
                obj.SetActive(currentDimension == Dimension.ThreeD);
            }
        }
    }
    
    /// <summary>
    /// 添加只在特定维度可见的物体
    /// </summary>
    public void AddDimensionSpecificObject(GameObject obj, Dimension visibleDimension)
    {
        if (obj == null) return;
        
        if (visibleDimension == Dimension.TwoD)
        {
            if (!onlyVisibleIn2D.Contains(obj))
            {
                onlyVisibleIn2D.Add(obj);
                
                // 从另一个列表移除（如果存在）
                if (onlyVisibleIn3D.Contains(obj))
                {
                    onlyVisibleIn3D.Remove(obj);
                }
                
                // 更新当前可见性
                obj.SetActive(currentDimension == Dimension.TwoD);
            }
        }
        else // ThreeD
        {
            if (!onlyVisibleIn3D.Contains(obj))
            {
                onlyVisibleIn3D.Add(obj);
                
                // 从另一个列表移除（如果存在）
                if (onlyVisibleIn2D.Contains(obj))
                {
                    onlyVisibleIn2D.Remove(obj);
                }
                
                // 更新当前可见性
                obj.SetActive(currentDimension == Dimension.ThreeD);
            }
        }
    }
}
