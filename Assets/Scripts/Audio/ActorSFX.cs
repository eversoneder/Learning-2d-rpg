using UnityEngine;

public class ActorSFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private bool isMute;

    public void PlaySFX(AudioClip audioClip)
    {
        isMute = AudioManager.instance.Mute;

        if (!isMute)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}
