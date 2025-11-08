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

    void Start()
    {
        // show REC on boot
        rewindUI?.SetRewindState(false);   
    }

    void Awake()
    {
        maxFrames = Mathf.CeilToInt(windowSeconds / Time.fixedDeltaTime);
    }

    void Update()
    {
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

        // set rewind ui
        rewindUI?.SetRewindState(true);

        // make a snapshot for ghost before we start rewinding
        List<Vector3> ghostFrames = new List<Vector3>(positions);

        // rewind player
        float timeToRewind = Mathf.Min(rewindSeconds, windowSeconds);
        int framesToRewind = Mathf.CeilToInt(timeToRewind / Time.fixedDeltaTime);

        // play positions backwards 
        int step = 2;
        for (int i = positions.Count - 1; i >= Mathf.Max(0, positions.Count - framesToRewind); i-= step)
        {
            player.position = positions[i];
            yield return new WaitForFixedUpdate();
        }

        // ghost spawn
        Transform ghost = Instantiate(ghostPrefab, player.position, Quaternion.identity);
        StartCoroutine(ReplayGhost(ghost, ghostFrames));

        // clear & resume recording
        positions.Clear();
        recording = true;

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
