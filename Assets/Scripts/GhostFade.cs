using UnityEngine;

public class GhostFade : MonoBehaviour
{
    Animator anim;
    [SerializeField] GameObject ownerToDestroy;
    [SerializeField] AudioClip ghostFadeSFX;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayFadeAndDestroy()
    {
        SFXManager.instance.PlaySFXclip(ghostFadeSFX, transform, 1f);
        if (anim)
            anim.SetTrigger("cloneFade");
    }

    // called by animation event
    public void DestroySelf()
    {
        Destroy(ownerToDestroy);
    }
}
