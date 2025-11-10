using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // --- Buttons ---
    [Header("Buttons / Conditions")]
    public ButtonTrigger button;
    [SerializeField] private List<ButtonTrigger> buttons = new List<ButtonTrigger>();
    [SerializeField] LevelManager levelManager;
    

    private bool doorOpen;
    public bool levelComplete;
    [SerializeField] private CameraManager cameraManager;

    [SerializeField] BoxCollider2D trigger;   // assign or GetComponent in Awake
    bool busy = false;


    public GameObject dropPoint;
    
    Animator animator;

    // sounds

    [SerializeField] AudioClip doorOpenSFX;
    [SerializeField] AudioClip doorCloseSFX;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        trigger = GetComponent<BoxCollider2D>();

    }


    private void Update()
    {
        // DEBUGGING
        if (Input.GetKeyDown(KeyCode.O)) button.pressed = true;
        if (Input.GetKeyDown(KeyCode.P)) button.pressed = false;




        if (button != null && buttons.Count > 0)
        {
            if (AllButtonsPressed())
            {
                OpenDoor();
                
            }

            else
            {
                CloseDoor();
                
            }
        }

        else
        {
            if (button != null)
            {
                if (button.pressed) OpenDoor();
                else CloseDoor();
            }
        }
        }

    void OpenDoor()
    {
        if (doorOpen) return;
        doorOpen = true;

        // play animation
        animator.SetBool("Open", true);
        // enable collider to allow door entrance
        GetComponent<BoxCollider2D>().enabled = true;

        SFXManager.instance.PlaySFXclip(doorOpenSFX, transform, 1f);
    }

    void CloseDoor()
    {

        if (!doorOpen) return;
        doorOpen = false;

        // play animation
        animator.SetBool("Open", false);
        // disable collider to prevent entrance
        GetComponent<BoxCollider2D>().enabled = false;

        SFXManager.instance.PlaySFXclip(doorCloseSFX, transform, 1f);
    }

    private bool AllButtonsPressed()
    {
        if (buttons == null || buttons.Count == 0) return false; // no multi set
        for (int i = 0; i < buttons.Count; i++)
        {
            var b = buttons[i];
            if (b == null || !b.pressed) return false;
        }
        return true;
    }





    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player") && doorOpen && !busy)
        {
            // prevents re-entry
            busy = true;
            if (trigger) trigger.enabled = false;


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

            




        }
    }

    IEnumerator DropSequence(PlayerController pc)
    {
        // already teleported to dropPoint & pc.FreezeNow() called
        var sr = pc.GetComponentInChildren<SpriteRenderer>();

        
        yield return DropFX.Play(pc.transform, sr, dropPoint.transform.position);

        levelComplete = true;

        // check if last level
        //if (levelManager.levelIndex >= levelManager.spawnPoints.Length - 1)
        //{
        //    // If it's the last level, go back to the very first camera (title)
        //    if (cameraManager != null) cameraManager.GoTo(0);

            
        //    pc.Freeze();
        //    LogPositions.canRewind = false;

        //    // optional: reset level index if you want to replay
        //    levelManager.levelIndex = 0;

        //    yield return new WaitForSeconds(0.5f);
        //    busy = false;
        //    if (trigger) trigger.enabled = true;
        //    yield break; // stop here — don't advance further
        //}

        // move to next level cam
        if (cameraManager != null) cameraManager.NextCam();

        // done
        pc.Unfreeze();
        levelComplete = false;
        
        levelManager.AdvanceLevel();

        yield return new WaitForSeconds(0.2f);  // slight delay to avoid overlap
        busy = false;
        if (trigger) trigger.enabled = true;


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

                if (target == null) yield break;

                target.position = Vector3.Lerp(peak, end, EaseInQuad(a));

                if (sr) sr.color = new Color(c.r, c.g, c.b, 1f - a);
                yield return null;
            }

            // final state: invisible & slightly below hole
            if (sr) sr.color = new Color(c.r, c.g, c.b, 0f);
        }
    }


    

}
