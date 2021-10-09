using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    private AudioSource audioSource;

     private static MusicManagerScript playerInstance;

    private void Awake()
    {
        DontDestroyOnLoad (this);
        if (playerInstance == null) {
            playerInstance = this;
            audioSource = GetComponent<AudioSource>();
            PlayMenuTheme();
        } else {
            Destroy(gameObject);
        }
    }

    public void PlayMenuTheme()
    {
        if (audioSource.clip != null)
            audioSource.Stop();

        audioSource.clip = (AudioClip)Resources.Load("Audio/MenuTrack");
        audioSource.Play();
    }
    
    public void PlayLevelTheme()
    {
        if (audioSource.clip != null)
        audioSource.Stop();

        audioSource.clip = (AudioClip)Resources.Load("Audio/LevelTrack");
        audioSource.Play();

    }
}
