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

    public bool inputLocked = false;

    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (inputLocked) return;

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
        if (inputLocked)
        {
            rb.linearVelocity = Vector2.zero;      
            if (anim) anim.SetFloat("Speed", 0f);
            return;                                 
        }

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

        //float speed = rb.linearVelocity.magnitude;
        anim.SetFloat("Speed", rb.linearVelocity.magnitude);

    }

    public void Freeze()
    {
        inputLocked = true;
        following = false;

        // kill all motion immediately
        rb.linearVelocity = Vector2.zero;
        velocityRef = Vector2.zero;

        // pause animation
        if (anim) anim.SetFloat("Speed", 0f);

        // stop physics from nudging us
        rb.bodyType = RigidbodyType2D.Kinematic;   
    }

    public void Unfreeze()
    {
        // restore physics
        rb.bodyType = RigidbodyType2D.Dynamic;

        // ensure we don't resume with leftover momentum
        rb.linearVelocity = Vector2.zero;
        velocityRef = Vector2.zero;

        inputLocked = false;
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
