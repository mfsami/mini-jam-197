using UnityEngine;

public class GhostReplay : MonoBehaviour
{
    private Vector3[] frames;
    private int index;
    private bool playing;
    //private float step = 0f;

    
    public void Init(Vector3[] recordedFrames)
    {
        frames = recordedFrames;
        index = 0;
        playing = frames != null && frames.Length > 0;

        if (playing)
        {
            transform.position = frames[0];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!playing) return;

        index++;
        if (index < frames.Length)
        {
            transform.position = frames[index];
        }
        else
        {
            // finished
            Destroy(gameObject);
        }
    }
}
