using UnityEngine;

public class Door : MonoBehaviour
{
    public ButtonTrigger button;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
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

    void OpenDoor()
    {
        // play animation
        animator.SetBool("Open", true);

        // enable collider to allow door entrance
        GetComponent<BoxCollider2D>().enabled = true;
    }

    void CloseDoor()
    {
        // play animation
        animator.SetBool("Open", false);

        // disable collider to prevent entrance
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
