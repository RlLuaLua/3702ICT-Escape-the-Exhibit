using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    public void ShowMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ShowHelp()
    {
        SceneManager.LoadScene("Help");
    }

    public void LoadLevel (string levelName)
    {
        GameObject.Find("MusicManager").GetComponent<MusicManagerScript>().PlayLevelTheme();
        SceneManager.LoadScene(levelName);
    }

    public void EndGame()
    {
        ShowMainMenu();
        GameObject.Find("MusicManager").GetComponent<MusicManagerScript>().PlayMenuTheme();
    }

    public void QuitGame()
    {
        Debug.Log("Quit application.");
        Application.Quit ();
    }
}
