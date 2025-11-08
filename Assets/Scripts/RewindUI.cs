using UnityEngine;
using UnityEngine.UI;

public class RewindUI : MonoBehaviour
{
    public Sprite recSprite;
    public Sprite rewindSprite;

    public Image recRewindImage;


    public void SetRewindState(bool isRewinding)
    {
        if (recRewindImage == null) return;

        recRewindImage.sprite = isRewinding ? rewindSprite : recSprite;
    }
}
