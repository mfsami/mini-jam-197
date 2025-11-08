using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    private SpriteRenderer sr;
    
    private float fadeSpeed = 2f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        
    }

    private void Update()
    {
        var c = sr.color;
        c.a -= fadeSpeed * Time.deltaTime;
        sr.color = c;

        if (c.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
