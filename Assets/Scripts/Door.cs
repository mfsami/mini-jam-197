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
            // StartCoroutine(FallAndFade(other.transform));
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
