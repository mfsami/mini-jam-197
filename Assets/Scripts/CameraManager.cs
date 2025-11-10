// CameraManager.cs
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] cams;

    [Header("Timings")]
    [SerializeField] private float menuWait = 5f;
    [SerializeField] private float blendSeconds = 1f;
    [SerializeField] private float descriptionDwell = 8f;

    [Header("Refs")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject UI;
    int i = 0;

    void Awake()
    {
        if (!playerController) playerController = FindFirstObjectByType<PlayerController>();
    }

    void OnEnable()
    {
        GoTo(0);
        //if (playerController) playerController.Freeze();
        StopAllCoroutines();
        StartCoroutine(StartFlow());
        //UI.SetActive(false);
        LogPositions.canRewind = false;       // lock until level start
    }

    public void GoTo(int index)
    {
        if (cams == null || cams.Length == 0 || index < 0 || index >= cams.Length) return;
        for (int k = 0; k < cams.Length; k++) cams[k].Priority = 10;
        cams[index].Priority = 20;
        i = index;
    }

    public void NextCam() => GoTo(i + 1);

    IEnumerator StartFlow()
    {

        // Disable rewind until level start
        


        yield return new WaitForSecondsRealtime(menuWait);

        GoTo(1);
        yield return new WaitForSecondsRealtime(blendSeconds + 0.1f);

        yield return new WaitForSecondsRealtime(descriptionDwell);

        GoTo(2);
        yield return new WaitForSecondsRealtime(blendSeconds + 0.1f);

        // Enable rewind once level starts
        LogPositions.canRewind = true;

        //UI.SetActive(true);
    }
}
