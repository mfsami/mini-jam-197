using UnityEngine;
using UnityEngine.UI;

public class UICursorFollow : MonoBehaviour
{
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false; // hide the OS cursor
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        rectTransform.position = mousePos;
    }
}
