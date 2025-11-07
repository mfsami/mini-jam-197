using UnityEngine;
using System.Collections.Generic;

public class LogPositions : MonoBehaviour
{
    public Transform player;
    public Transform ghostPrefab;
    public Transform originPoint;

    public float windowSeconds = 20f;
    private int maxFrames;

    private List<Vector3> positions = new List<Vector3>();
    private bool recording = true;

    void Awake()
    {
        maxFrames = Mathf.CeilToInt(windowSeconds / Time.fixedDeltaTime);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && positions.Count > 0)
        {
            // Copy current positions for this ghost
            List<Vector3> ghostFrames = new List<Vector3>(positions);

            // Spawn ghost at player's position (or origin, if you prefer)
            Transform ghost = Instantiate(ghostPrefab, player.position, Quaternion.identity);

            // Start coroutine so this ghost replays its own copy
            StartCoroutine(ReplayGhost(ghost, ghostFrames));

            // Teleport player back to origin
            player.position = originPoint.position;

            // Clear list and resume recording fresh
            positions.Clear();
            recording = true;
        }
    }

    void FixedUpdate()
    {
        if (!recording) return;

        // Record positions 
        positions.Add(player.position);

        // Keep only last 20 seconds
        if (positions.Count > maxFrames)
            positions.RemoveAt(0);
    }

    private System.Collections.IEnumerator ReplayGhost(Transform ghost, List<Vector3> frames)
    {
        // Move ghost along recorded path, one frame per FixedUpdate
        for (int i = 0; i < frames.Count; i++)
        {
            if (ghost == null) yield break;
            ghost.position = frames[i];
            yield return new WaitForFixedUpdate();
        }

        // Destroy after replay finishes
        if (ghost != null)
            Destroy(ghost.gameObject);
    }
}
