using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] cams;

    [Header("Timings")]
    [SerializeField] private float menuWait = 5f;       // wait on menu before moving
    [SerializeField] private float blendSeconds = 1f;
    [SerializeField] private float descriptionDwell = 8f;

    [Header("Refs")]
    [SerializeField] private PlayerController playerController;

    private int i = 0;

    private void OnEnable()
    {
        GoTo(0); // start on menu
        if (playerController) playerController.inputLocked = true;
        StopAllCoroutines();
        StartCoroutine(StartFlow());
    }

    public void GoTo(int index)
    {
        if (cams == null || cams.Length == 0 || index < 0 || index >= cams.Length)
        {
            Debug.LogWarning($"CameraManager.GoTo({index}) ignored. camsLen={cams?.Length ?? 0}");
            return;
        }

        // lower all
        for (int k = 0; k < cams.Length; k++) cams[k].Priority = 10;

        // raise desired
        cams[index].Priority = 20;
        i = index;
    }

    public void NextCam() => GoTo(i + 1);

    private IEnumerator StartFlow()
    {
        // Sit on Menu
        yield return new WaitForSecondsRealtime(menuWait);

        // Menu (0) -> Description (1)
        GoTo(1);
        yield return new WaitForSecondsRealtime(blendSeconds + 0.1f);

        // Sit on Description
        yield return new WaitForSecondsRealtime(descriptionDwell);

        // Description (1) -> Level 1 (2)
        GoTo(2);
        yield return new WaitForSecondsRealtime(blendSeconds + 0.1f);

        // Unlock controls for gameplay
        if (playerController) playerController.inputLocked = false;
    }
}
