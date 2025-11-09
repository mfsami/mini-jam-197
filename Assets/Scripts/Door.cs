using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public ButtonTrigger button;
    private bool doorOpen;
    

    public GameObject dropPoint;
    
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }


    private void Update()
    {
        // DEBUGGING
        if (Input.GetKeyDown(KeyCode.O)) button.pressed = true;
        if (Input.GetKeyDown(KeyCode.P)) button.pressed = false;


        if (button != null)
        {
            if (button.pressed)
            {
                OpenDoor();
            }

            else
            {
                CloseDoor();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player") && doorOpen)
        {
            Debug.Log("NICE WORK BOY!");

            //disable player inputs
            var controller = other.GetComponent<PlayerController>();

            if (controller != null)
            {
                controller.inputLocked = true; 
            }

            // drop animation
            other.transform.position = dropPoint.transform.position;


            // lower positin and lower opactity
            var pc = other.GetComponent<PlayerController>();
            if (pc == null) return;

            pc.Freeze();                                   // no gliding
            other.transform.position = dropPoint.transform.position;    // snap
            StartCoroutine(DropSequence(pc));
            Destroy(pc);

        }
    }

    IEnumerator DropSequence(PlayerController pc)
    {
        // already teleported to dropPoint & pc.FreezeNow() called
        var sr = pc.GetComponentInChildren<SpriteRenderer>();

        // Isaac-like sequence
        yield return DropFX.Play(pc.transform, sr, dropPoint.transform.position);

        // done — either load next scene here or unfreeze & respawn elsewhere
        pc.Unfreeze();
    }

    public static class DropFX
    {
        static float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);
        static float EaseInQuad(float t) => t * t;

        public static IEnumerator Play(Transform target, SpriteRenderer sr,
                                       Vector3 holePos,
                                       float hopHeight = 0.35f,
                                       float hopTime = 0.12f,
                                       float holdTime = 0.15f,
                                       float fallDepth = 0.6f,
                                       float fallTime = 0.7f)
        {
            if (!sr) sr = target.GetComponentInChildren<SpriteRenderer>();

            // --- start from hole position (no re-snap mid animation) ---
            target.position = new Vector3(holePos.x, holePos.y, target.position.z);

            Vector3 start = target.position;
            Vector3 peak = start + Vector3.up * hopHeight;
            Vector3 end = start + Vector3.down * fallDepth;

            Color c = sr ? sr.color : Color.white;
            float t = 0f;

            // --- hop up ---
            while (t < hopTime)
            {
                t += Time.deltaTime;
                float a = Mathf.Clamp01(t / hopTime);
                target.position = Vector3.Lerp(start, peak, EaseOutQuad(a));
                yield return null;
            }

            // --- small hold ---
            yield return new WaitForSeconds(holdTime);

            // smal scale
            //float scale = Mathf.Lerp(1f, 0.6f, a);
            //target.localScale = new Vector3(scale, scale, 1f);

            // --- fall down + fade ---
            t = 0f;
            while (t < fallTime)
            {
                t += Time.deltaTime;
                float a = Mathf.Clamp01(t / fallTime);
                target.position = Vector3.Lerp(peak, end, EaseInQuad(a));

                if (sr) sr.color = new Color(c.r, c.g, c.b, 1f - a);
                yield return null;
            }

            // final state: invisible & slightly below hole
            if (sr) sr.color = new Color(c.r, c.g, c.b, 0f);
        }
    }


    void OpenDoor()
    {
        // play animation
        animator.SetBool("Open", true);

        doorOpen = true;

        // enable collider to allow door entrance
        GetComponent<BoxCollider2D>().enabled = true;
    }

    void CloseDoor()
    {
        // play animation
        animator.SetBool("Open", false);

        doorOpen = false;

        // disable collider to prevent entrance
        GetComponent<BoxCollider2D>().enabled = false;
    }

}
