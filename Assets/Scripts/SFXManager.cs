using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private AudioSource sfxObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySFXclip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // spawn game object
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);

        //assign clip
        audioSource.clip = audioClip;

        // volume
        audioSource.volume = volume;

        // play sound
        audioSource.Play();

        // get length of sfx clip
        float clipLength = audioSource.clip.length;

        // destroy clip after its done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
