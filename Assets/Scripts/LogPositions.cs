using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPositions : MonoBehaviour
{
    public Transform player;
    public Transform ghostPrefab;
    public Transform originPoint;
    public RewindUI rewindUI;

    public float windowSeconds = 10f; // recording window
    public float rewindSeconds = 10f; // how far back to rewind

    private int maxFrames;
    private List<Vector3> positions = new List<Vector3>();
    private bool recording = true;
    private bool canRewind = true;

    // UI Stuff
    private float timerSeconds = 0f;
    private bool isRewinding = false;

    void Start()
    {
        // show REC on boot
        rewindUI?.SetRewindState(false);
        rewindUI?.SetTimerSeconds(timerSeconds);
    }

    void Awake()
    {
        maxFrames = Mathf.CeilToInt(windowSeconds / Time.fixedDeltaTime);
    }

    void Update()
    {

        // count up continuously while not rewinding
        if (!isRewinding)
        {
            timerSeconds += Time.deltaTime;
            rewindUI?.SetTimerSeconds(timerSeconds);
        }


        if (Input.GetMouseButtonDown(1) && positions.Count > 0 && canRewind)
        {

            StartCoroutine(RewindAndSpawnGhost());

        }
    }

    void FixedUpdate()
    {
        if (!recording) return;

        // record positions 
        positions.Add(player.position);

        // keep only last 20 seconds
        if (positions.Count > maxFrames)
            positions.RemoveAt(0);
    }

    private IEnumerator RewindAndSpawnGhost()
    {
        canRewind = false;
        recording = false;
        isRewinding = true;

        // set rewind ui
        rewindUI?.SetRewindState(true);

        // make a snapshot for ghost before we start rewinding
        List<Vector3> ghostFrames = new List<Vector3>(positions);

        // how much we can actually rewind (can’t go before time 0)
        float available = Mathf.Min(windowSeconds, timerSeconds); 
        float want = Mathf.Min(rewindSeconds, available);
        int framesToRewind = Mathf.CeilToInt(want / Time.fixedDeltaTime);

        // target time = (now - want); we’ll count DOWN to this while rewinding
        float targetTime = timerSeconds - want;
        int step = 2;                       
        float perStepSeconds = step * Time.fixedDeltaTime;

        // play positions backwards and decrement timer
        for (int i = positions.Count - 1; i >= Mathf.Max(0, positions.Count - framesToRewind); i-= step)
        {
            player.position = positions[i];

            // countdown the UI timer toward target
            timerSeconds = Mathf.Max(targetTime, timerSeconds - perStepSeconds);
            rewindUI?.SetTimerSeconds(timerSeconds);

            yield return new WaitForFixedUpdate();
        }

        // ghost spawn
        Transform ghost = Instantiate(ghostPrefab, player.position, Quaternion.identity);
        StartCoroutine(ReplayGhost(ghost, ghostFrames));

        // clear & resume recording
        positions.Clear();
        recording = true;
        isRewinding = false;

        // set back to rec
        rewindUI?.SetRewindState(false);

        canRewind = true;
    }

    private IEnumerator ReplayGhost(Transform ghost, List<Vector3> frames)
    {
        // play the ghost forward
        for (int i = 0; i < frames.Count; i++)
        {
            if (ghost == null) yield break;
            ghost.position = frames[i];
            yield return new WaitForFixedUpdate();
        }

        if (ghost != null)
            // wait a bit before immedatley destroying
            yield return new WaitForSeconds(2);
            Destroy(ghost.gameObject);
    }
}
