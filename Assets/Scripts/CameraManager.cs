using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineCamera[] cams;

    int i = 0;

    private void OnEnable()
    {
        GoTo(0); // guard for level 1 cam
    }

    public void GoTo(int index)
    {
        if (cams == null || index < 0 || index >= cams.Length) return;

        // lower all
        for (int k = 0; k < cams.Length; k++)
            cams[k].Priority = 10;

        // raise the one we want
        cams[index].Priority = 20;
        i = index;
    }

    public void NextCam() => GoTo(i + 1);
}
