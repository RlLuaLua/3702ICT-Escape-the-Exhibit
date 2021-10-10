using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    public static GUIManager shared;
    private AudioSource audioSource;

    public int level = 0;

    private void Awake()
    {
        DontDestroyOnLoad (this);
        if (shared == null) {
            shared = this;
            audioSource = GetComponent<AudioSource>();
            PlayMenuTheme();
        } else {
            Destroy(gameObject);
        }
    }

    public void ShowMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowNextLevel()
    { 
        if (NextLevelExists()) {
            if (level == 0)
            {
                PlayLevelTheme();
            }
            level++;
            SceneManager.LoadScene(LevelNameForLevel(level));
        }
    }

    public void RestartGame()
    {
        level = 0;
        ShowNextLevel();
    }

    public bool NextLevelExists()
    { 
        return Application.CanStreamedLevelBeLoaded(LevelNameForLevel(level + 1));
    }

    string LevelNameForLevel(int level)
    {
        return "Level " + level.ToString();
    }

    public void EndGame()
    {
        level = 0;
        PlayMenuTheme();
        ShowMainMenu();
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
