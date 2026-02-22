using UnityEngine;

/// <summary>
/// Second Map Background Music Player
/// </summary>
public class AudioControll : MonoBehaviour
{
    [SerializeField] private AudioClip bgmMusic;

    private AudioManager audioManager;

    void Start()
    {
        if (AudioManager.instance != null)
        {
            audioManager = AudioManager.instance;
            audioManager.PlayBGM(bgmMusic);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found.");
        }
    }
}