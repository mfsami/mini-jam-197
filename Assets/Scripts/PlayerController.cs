using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public float stepSize = 1f;
    private bool canMove = true;
  


    private void Start()
    {
        // detatch from parent
        movePoint.parent = null;
        
    }

    private void Update()
    {
        // move towards movepoint
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        
        // only read new input when at new point
        if (canMove && Vector3.Distance(transform.position, movePoint.position) <= 0.05f && Input.GetMouseButtonDown(0))
        {
            // mouse in world space
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = transform.position.z;

            Vector2 delta = mouseWorld - transform.position;

            // choose dominant axis
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                movePoint.position += new Vector3(Mathf.Sign(delta.x) * stepSize, 0f, 0f);
                StartCoroutine(MoveTimer(0.5f));
            }

            else
            {
                movePoint.position += new Vector3(0f, Mathf.Sign(delta.y) * stepSize, 0f);
                StartCoroutine(MoveTimer(0.5f));
            }
        }
    }

    private IEnumerator MoveTimer(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }
}
