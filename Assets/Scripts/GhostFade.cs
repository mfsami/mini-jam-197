using UnityEngine;

public class GhostFade : MonoBehaviour
{
    Animator anim;
    [SerializeField] GameObject ownerToDestroy;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayFadeAndDestroy()
    {
        if (anim)
            anim.SetTrigger("cloneFade");
    }

    // called by animation event
    public void DestroySelf()
    {
        Destroy(ownerToDestroy);
    }
}
