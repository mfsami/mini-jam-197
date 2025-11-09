using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    public bool pressed = false;

    private void Start()
    {
        if (inactiveSprite == null && spriteRenderer != null)
            inactiveSprite = spriteRenderer.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = activeSprite;   // swap sprite
            pressed = true;
            
        }

        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spriteRenderer.sprite = inactiveSprite;   // swap sprite
            pressed = false;
            
        }
    }
}
