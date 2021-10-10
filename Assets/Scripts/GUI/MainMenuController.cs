using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private GUIManager manager;

    void Start()
    {
        manager = GameObject.Find("GUIManager").GetComponent<GUIManager>();
    }

    public void Play()
    {
        manager.ShowNextLevel();
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ShowHelp()
    {
        SceneManager.LoadScene("Help");
    }

    public void QuitGame()
    {
        Debug.Log("Quit application.");
        Application.Quit ();
    }

}
