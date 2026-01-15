using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;
    
    [Header("Music Settings")]
    public AudioClip backgroundMusic;
    public float volume = 0.3f;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        // Singleton pattern - only one music manager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persists between scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicates
            return;
        }
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }
    
    // Optional: Change music
    public void ChangeMusic(AudioClip newMusic)
    {
        if (newMusic != null)
        {
            audioSource.Stop();
            audioSource.clip = newMusic;
            audioSource.Play();
        }
    }
    
    // Optional: Control volume
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
    
    // Optional: Mute/unmute
    public void ToggleMute()
    {
        audioSource.mute = !audioSource.mute;
    }
}