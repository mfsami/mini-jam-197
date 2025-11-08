using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewindUI : MonoBehaviour
{
    public Sprite recSprite;
    public Sprite rewindSprite;
    public TMP_Text timer;
    public Image recRewindImage;


    public void SetRewindState(bool isRewinding)
    {
        if (recRewindImage == null) return;

        recRewindImage.sprite = isRewinding ? rewindSprite : recSprite;
    }


    public void SetTimerSeconds(float seconds)
    {
        if (!timer) return;
        // Show one decimal
        timer.text = seconds.ToString("0.0") + "s";
    }
}
