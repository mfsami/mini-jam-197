using UnityEngine;

public class GhostController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;             // Animator on child (PlayerSprite)
    private SpriteRenderer sr;         // SpriteRenderer on child
    private Vector3 lastPos;


    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr   = GetComponentInChildren<SpriteRenderer>();

        lastPos = transform.position;
    }

    void LateUpdate()
    {
        
        float rbSpeed = (rb && rb.simulated) ? rb.linearVelocity.magnitude : 0f;

        Vector3 delta = transform.position - lastPos;

        // Flip similar to PlayerController
        float dx = (rbSpeed > 0.0001f) ? rb.linearVelocity.x : delta.x;
        if (sr)
        {
            if (dx > 0.05f)      sr.flipX = false;
            else if (dx < -0.05f) sr.flipX = true;
        }

        anim.SetFloat("Speed", rbSpeed);



        lastPos = transform.position;
    }
}
