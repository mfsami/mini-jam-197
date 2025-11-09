using UnityEngine;

public class GhostController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;             // Animator on child (PlayerSprite)
    private SpriteRenderer sr;         // SpriteRenderer on child
    private Vector3 lastPos;

    static readonly int SpeedHash = Animator.StringToHash("Speed");
    float logTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (!anim) Debug.LogError("[GhostController] No Animator found in children.");
        if (!sr) Debug.LogWarning("[GhostController] No SpriteRenderer found in children.");

        lastPos = transform.position;
    }

    void LateUpdate()
    {
        // if physics is used, this will be > 0; otherwise we’ll fall back to delta
        float rbSpeed = (rb && rb.simulated) ? rb.linearVelocity.magnitude : 0f;

        Vector3 delta = transform.position - lastPos;
        float deltaSpeed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

        float speed = (rbSpeed > 0.0001f) ? rbSpeed : deltaSpeed;

        if (anim) anim.SetFloat(SpeedHash, speed);

        // Flip similar to PlayerController
        float dx = (rbSpeed > 0.0001f) ? rb.linearVelocity.x : delta.x;
        if (sr)
        {
            if (dx > 0.05f) sr.flipX = false;
            else if (dx < -0.05f) sr.flipX = true;
        }

        // debug print 4x/sec so you can see the value in console
        logTimer += Time.deltaTime;
        if (logTimer > 0.25f)
        {
            if (anim) Debug.Log($"[GhostController] Speed -> {speed:F3}");
            logTimer = 0f;
        }

        lastPos = transform.position;
    }
}
