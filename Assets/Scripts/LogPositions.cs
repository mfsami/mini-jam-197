using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPositions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform ghostPrefab;
    [SerializeField] private RewindUI rewindUI;
    [SerializeField] private CloneHeadsUI cloneHeadsUI;

    [Header("Rewind Settings")]
    [SerializeField] private float windowSeconds = 5f;
    [SerializeField] private float rewindSeconds = 5f;
    [SerializeField] private int ghostCap = 3;

    [Header("Trail Settings")]
    [SerializeField] private GameObject ghostTrailPrefab;
    [SerializeField] private float ghostSpawnRate = 0.05f;

    private readonly List<Vector3> positions = new();
    private float timerSeconds;
    private float ghostTimer;
    private int maxFrames;
    private int activeGhosts;
    private bool recording = true;
    private bool canRewind = true;
    private bool isRewinding;

    private static readonly Color[] TrailColors =
    {
        new(1f, 0.31f, 0.64f, 0.47f),
        new(0f, 1f, 1f, 0.47f),
        new(0.64f, 0.42f, 1f, 0.47f),
        new(1f, 0.84f, 0.04f, 0.47f),
        new(0.25f, 1f, 0.5f, 0.47f)
    };

    void Awake()
    {
        maxFrames = Mathf.CeilToInt(windowSeconds / Time.fixedDeltaTime);
        if (!player) player = transform;
        if (!rewindUI) rewindUI = FindFirstObjectByType<RewindUI>();
        if (!cloneHeadsUI) cloneHeadsUI = FindFirstObjectByType<CloneHeadsUI>();

    }

    void Start()
    {
        rewindUI?.SetRewindState(false);
        rewindUI?.SetTimerSeconds(timerSeconds);
        cloneHeadsUI?.SetCount(0);
    }

    void Update()
    {
        if (!isRewinding)
        {
            timerSeconds += Time.deltaTime;
            rewindUI?.SetTimerSeconds(timerSeconds);
        }

        if (Input.GetMouseButtonDown(1) && positions.Count > 0 && canRewind && activeGhosts < ghostCap)
            StartCoroutine(RewindAndSpawnGhost());
    }

    void FixedUpdate()
    {
        if (!recording) return;
        positions.Add(player.position);
        if (positions.Count > maxFrames) positions.RemoveAt(0);
    }

    private IEnumerator RewindAndSpawnGhost()
    {
        var controller = player.GetComponent<PlayerController>();
        if (controller) controller.inputLocked = true;

        canRewind = false;
        recording = false;
        isRewinding = true;
        rewindUI?.SetRewindState(true);

        float available = Mathf.Min(windowSeconds, timerSeconds);
        float want = Mathf.Min(rewindSeconds, available);
        int framesToRewind = Mathf.CeilToInt(want / Time.fixedDeltaTime);

        int start = Mathf.Max(0, positions.Count - framesToRewind);
        var ghostFrames = positions.GetRange(start, positions.Count - start);
        float targetTime = timerSeconds - want;
        float stepTime = 2 * Time.fixedDeltaTime;

        for (int i = positions.Count - 1; i >= start; i -= 2)
        {
            player.position = positions[i];
            SpawnTrail();
            timerSeconds = Mathf.Max(targetTime, timerSeconds - stepTime);
            rewindUI?.SetTimerSeconds(timerSeconds);
            yield return new WaitForFixedUpdate();
        }

        // spawn ghost
        Transform ghost = Instantiate(ghostPrefab, ghostFrames[0], Quaternion.identity);
        activeGhosts++;
        cloneHeadsUI?.SetCount(activeGhosts);
        StartCoroutine(ReplayGhost(ghost, ghostFrames));

        positions.Clear();
        recording = true;
        isRewinding = false;
        rewindUI?.SetRewindState(false);
        if (controller) controller.inputLocked = false;
        canRewind = true;
    }

    private void SpawnTrail()
    {
        ghostTimer += Time.deltaTime;
        if (ghostTimer < ghostSpawnRate) return;
        var trail = Instantiate(ghostTrailPrefab, player.position, Quaternion.identity);
        trail.GetComponent<SpriteRenderer>().color = TrailColors[Random.Range(0, TrailColors.Length)];
        ghostTimer = 0f;
    }

    private IEnumerator ReplayGhost(Transform ghost, List<Vector3> frames)
    {
        foreach (var frame in frames)
        {
            if (!ghost) yield break;
            ghost.position = frame;
            yield return new WaitForFixedUpdate();
        }

        if (!ghost) yield break;
        yield return new WaitForSeconds(4);

        var fade = ghost.GetComponentInChildren<GhostFade>();
        if (fade) fade.PlayFadeAndDestroy();
        else Destroy(ghost.gameObject);

        activeGhosts = Mathf.Max(0, activeGhosts - 1);
        cloneHeadsUI?.SetCount(activeGhosts);
    }
}
