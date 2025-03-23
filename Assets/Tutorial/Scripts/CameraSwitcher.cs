using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.Rendering;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cinemachine Cameras")]  
    public CinemachineCamera threeDCam;
    public CinemachineCamera twoDCam;

    [Header("Camera Blend Settings")]
    public CinemachineBlendDefinition blend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 2f);

    [Header("Player Reference")]
    public PlayerMovement player;

    public bool isThreeD = false;

    private Camera mainCamera;

    public Volume threeDVolume;
    public Volume twoDVolume;

    [Tooltip("Duration for post-processing volume transition")]
    public float transitionDuration = 2f;

    private Coroutine transitionCoroutine;

    void Start()
    {
        isThreeD = false;

        threeDCam.Priority = 5;
        twoDCam.Priority = 10;

        threeDVolume.weight = 0f;
        twoDVolume.weight = 1f;

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("CameraSwitcher: 无法找到 Main Camera！");
        }
        else
        {
            var brain = mainCamera.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                brain.DefaultBlend = blend;
            }
        }

        // 初始化设置为2D视角，锁定Z轴
        if (player != null)
        {
            player.SetZMovementEnabled(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleCameraView();
        }
    }

    void ToggleCameraView()
    {
        if (isThreeD)
        {
            threeDCam.Priority = 5;
            twoDCam.Priority = 10;
        }
        else
        {
            threeDCam.Priority = 10;
            twoDCam.Priority = 5;
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionVolume(isThreeD));
        isThreeD = !isThreeD;

        if (player != null)
        {
            player.SetZMovementEnabled(isThreeD);
        }
    }

    IEnumerator TransitionVolume(bool toFirstPerson)
    {
        float elapsedTime = 0f;
        float startWeightFP = threeDVolume.weight;
        float startWeightTD = twoDVolume.weight;
        float targetWeightFP = toFirstPerson ? 0f : 1f;
        float targetWeightTD = toFirstPerson ? 1f : 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            threeDVolume.weight = Mathf.Lerp(startWeightFP, targetWeightFP, t);
            twoDVolume.weight = Mathf.Lerp(startWeightTD, targetWeightTD, t);

            yield return null;
        }

        threeDVolume.weight = targetWeightFP;
        twoDVolume.weight = targetWeightTD;
        transitionCoroutine = null;
    }
}