using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.Rendering;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera threeDCam;
    public CinemachineCamera twoDCam;

    private bool isThreeD = false;

    private Camera mainCamera;

    public Volume threeDVolume;
    public Volume twoDVolume;
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
