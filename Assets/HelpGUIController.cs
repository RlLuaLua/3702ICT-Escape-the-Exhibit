using UnityEngine;
using UnityEngine.UI;

public class HelpGUIController : MonoBehaviour
{
    public Image currentPageImage;
    public Sprite[] pages;
    public Button backButton;
    public Button forwardButton;

    private int currentPage = 0;

    void Start()
    {
        UpdateUI();
    }

    public void GoBack()
    {
        currentPage--;
        UpdateUI();
    }

    public void GoForward()
    {
        currentPage++;
        UpdateUI();
    }

    void UpdateUI()
    {
        backButton.gameObject.SetActive(currentPage != 0);
        forwardButton.gameObject.SetActive(!(currentPage == (pages != null ? (pages.Length - 1) : 0)));

        if (pages != null)
            currentPageImage.sprite = pages[currentPage];
    }
}
