using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;          // top speed
    public float smoothTime = 0.12f;      // higher = more delay/laggy follow
    public float stopRadius = 0.6f;      // don't jitter when very close

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 velocityRef;          // used by SmoothDamp
    private Vector2 targetPos;            // world-space target (mouse)
    private bool following;               // only follow while holding LMB

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // Update target continuously while holding LMB
        if (Input.GetMouseButton(0))
        {
            following = true;
            Vector3 m = GetMouseWorldOnPlayerPlane();
            targetPos = new Vector2(m.x, m.y);
        }
        else
        {
            following = false;
        }

        // Flip sprite based on left and right since same anim
        if (rb.linearVelocity.x > 0.05f)
            sr.flipX = false;
        else if (rb.linearVelocity.x < -0.05f)
            sr.flipX = true;
    }

    void FixedUpdate()
    {
        Vector2 pos = rb.position;

        Vector2 desiredVel = Vector2.zero;

        if (following)
        {
            Vector2 toTarget = targetPos - pos;
            if (toTarget.sqrMagnitude > stopRadius * stopRadius)
            {
                desiredVel = toTarget.normalized * moveSpeed;
            }
        }

        // SmoothDamp current velocity toward the desired velocity
        // slight delay
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, desiredVel, ref velocityRef, smoothTime);

        
        
    }

    Vector3 GetMouseWorldOnPlayerPlane()
    {
        Camera cam = Camera.main;
        if (cam.orthographic)
        {
            Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
            p.z = transform.position.z;        // keep same Z as player
            return p;
        }
        else
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            float t = (transform.position.z - r.origin.z) / r.direction.z;
            return r.origin + r.direction * t;
        }
    }
}
