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

    public void LoadLevel (string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit application.");
        Application.Quit ();
    }
}
