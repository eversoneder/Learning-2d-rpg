using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Audio manager that uses a pool of AudioSource components, robust and scalable solution <br></br>for handling sound effects without overlapping or mixing issues causing loss of sound quality.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmAudioSource;

    [Header("SFX Pooling")]
    [SerializeField] private GameObject sfxSourcePrefab;
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private bool mute;

    private float startPitch = 1.2f;
    private float endPitch = 0.8f;
    private float fadeDuration = 0.5f;


    private List<AudioSource> sfxSources = new List<AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize the object pool for SFX
            InitializeSfxPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (mute == true)
        {
            bgmAudioSource.volume = 0.0f;
        } else
        {
            bgmAudioSource.volume = 0.1f;
        }
    }

    private void InitializeSfxPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewSfxSource();
        }
    }

    private AudioSource CreateNewSfxSource()
    {
        GameObject newSfxObject = Instantiate(sfxSourcePrefab, transform);
        AudioSource newSfxSource = newSfxObject.GetComponent<AudioSource>();
        sfxSources.Add(newSfxSource);
        return newSfxSource;
    }

    public void PlayBGM(AudioClip audioClip)
    {
        if (!mute)
        {
            bgmAudioSource.clip = audioClip;
            bgmAudioSource.Play();
        }

    }

    public void PlaySFX(AudioClip audioClip)
    {
        if (!mute)
        {
            // Find an available AudioSource from the pool
            AudioSource availableSource = FindAvailableSfxSource();
            if (availableSource != null)
            {
                availableSource.PlayOneShot(audioClip);
            }
            else
            {
                Debug.LogWarning("No available SFX source in the pool. Instantiating a new one.");
                AudioSource newSource = CreateNewSfxSource();
                newSource.PlayOneShot(audioClip);
            }
        }
    }

    // Inside your AudioManager.cs script
    public void PlaySFXWithPitch(AudioClip audioClip, float pitch)
    {
        if (!mute)
        {
            AudioSource availableSource = FindAvailableSfxSource();
            if (availableSource != null)
            {
                availableSource.pitch = pitch;
                availableSource.PlayOneShot(audioClip);
            }
            else
            {
                Debug.LogWarning("No available SFX source. Instantiating a new one.");
                AudioSource newSource = CreateNewSfxSource();
                newSource.pitch = pitch;
                newSource.PlayOneShot(audioClip);
            }
        }
    }

    public void PlayDeathSFX(AudioClip audioClip)
    {

        if (!mute)
        {
            AudioSource availableSource = FindAvailableSfxSource();
            if (availableSource != null)
            {
                availableSource.pitch = 1.4f;
                availableSource.PlayOneShot(audioClip);
            }
            else
            {
                Debug.LogWarning("No available SFX source. Instantiating a new one.");
                AudioSource newSource = CreateNewSfxSource();
                newSource.pitch = 1.4f;
                newSource.PlayOneShot(audioClip);
            }
        }
    }

    private IEnumerator PlayAndFadePitch(AudioClip audioClip)
    {
        AudioSource availableSource = FindAvailableSfxSource();
        availableSource.clip = audioClip;
        availableSource.pitch = startPitch;
        availableSource.Play();

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // Lerp pitch from start to end
            availableSource.pitch = Mathf.Lerp(startPitch, endPitch, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure pitch is set exactly to endPitch at the end
        availableSource.pitch = endPitch;
    }

    private AudioSource FindAvailableSfxSource()
    {
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null;
    }

    public bool Mute { get => mute; set => mute = value; }
}