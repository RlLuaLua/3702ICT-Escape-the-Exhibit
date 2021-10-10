using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    enum LevelState
    {
        Playing,
        Won,
        Lost
    }
    
    public int level;
    private LevelState state;
    private GUIManager manager;

    //Alert canvas (in-front of scene elements)
    [SerializeField] private Canvas alertCanvas;
    [SerializeField] private Image gameStateImage;
    [SerializeField] private Text alertText;

    //Background canvas (behind scene elements)
    [SerializeField] private Image paintingImage;
    Vector3[] paintingBounds = new Vector3[4];
    [SerializeField] private Image healthMask;
    [SerializeField] private Text enemiesText;
    [SerializeField] private Text levelText;

    private HealthController playerHealthController;
    private GameObject checkpoint;
    private GameObject[] enemies;
    private GameObject[] images;

    private int aliveEnemies;
    private float healthMaskMaxWitdth;
    private float pictureSpawnInset = 100.0f;

    void Start()
    {
        if (GameObject.Find("GUIManager"))
        {
            manager = GameObject.Find("GUIManager").GetComponent<GUIManager>();
        }

        healthMaskMaxWitdth = healthMask.rectTransform.sizeDelta.x;
        paintingImage.rectTransform.GetLocalCorners(paintingBounds);

        Reset();
    }

    void Reset()
    {
        state = LevelState.Playing;
        Texture2D paintingTexture = Resources.Load<Texture2D>("Levels/" + level.ToString());
        paintingImage.sprite = Sprite.Create(paintingTexture, new Rect(0, 0, paintingTexture.width, paintingTexture.height), new Vector2(0.5f, 0.5f));

        var player = GameObject.FindGameObjectWithTag("Player");
        playerHealthController = player.GetComponent<HealthController>();
        checkpoint = GameObject.FindGameObjectWithTag("Checkpoint");
        checkpoint.SetActive(false);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        images = new GameObject[enemies.Length];

        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject imgObject = new GameObject("InkSplatter");

            RectTransform trans = imgObject.AddComponent<RectTransform>();
            trans.transform.SetParent(paintingImage.transform);
            trans.localScale = Vector3.one;
            trans.anchoredPosition3D = new Vector3(
                Random.Range(paintingBounds[0].x + pictureSpawnInset, paintingBounds[2].x - pictureSpawnInset),
                Random.Range(paintingBounds[0].y + pictureSpawnInset, paintingBounds[1].y - pictureSpawnInset),
                0f
            );
            trans.sizeDelta = new Vector2(300, 300);
            trans.Rotate(new Vector3(0,0,Random.rotation.eulerAngles.z));

            Image image = imgObject.AddComponent<Image>();
            Texture2D tex = Resources.Load<Texture2D>("Ink");
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            imgObject.transform.SetParent(paintingImage.transform);

            images[i] = imgObject;
        }

    }

    void Update()
    {
        switch (state)
        {
            case LevelState.Playing: UpdatePlayingState(); break;
            case LevelState.Won: UpdateWonState(); break;
            case LevelState.Lost: UpdateLostState(); break;
        }
    }

    void UpdatePlayingState()
    {
        playerHealthController.gameObject.SetActive(true);

        // alertCanvas.gameObject.SetActive(false);

        aliveEnemies = enemies.Length;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) {
                Destroy(images[i]);
                aliveEnemies--;
            }
        }

        enemiesText.text = "Enemies: " + aliveEnemies;
        levelText.text = "Level: " + level.ToString();

        healthMask.rectTransform.sizeDelta = new Vector2(healthMaskMaxWitdth / playerHealthController.max_health * playerHealthController.cur_health, healthMask.rectTransform.sizeDelta.y);

        if (aliveEnemies == 0)
        {
            checkpoint.SetActive(true);
        }

        if (playerHealthController.cur_health <= 0)
        {
            state = LevelState.Lost;
        }
    }

    void UpdateWonState()
    {
        playerHealthController.gameObject.SetActive(false);
        alertText.text = "Nice work! The exhibition will go ahead! Want to go back in time and do it again?";
        gameStateImage.sprite = loadSpriteForWin(true);
        alertCanvas.gameObject.SetActive(true);  
    }

    void UpdateLostState()
    {
        playerHealthController.gameObject.SetActive(false);        
        alertText.text = "Now the exhibition will have to be cancelled! Would you like to try again?";
        gameStateImage.sprite = loadSpriteForWin(false);
        alertCanvas.gameObject.SetActive(true);
    }

    Sprite loadSpriteForWin(bool win)
    {
        Texture2D paintingTexture = Resources.Load<Texture2D>("UI/" + (win ? "Win" : "Loss") + "Text");
        return Sprite.Create(paintingTexture, new Rect(0, 0, paintingTexture.width, paintingTexture.height), new Vector2(0.5f, 0.5f));
    }

    void NextLevel()
    {
        // level++;
        manager?.ShowNextLevel();
        Reset();
    }

    public void PlayAgain()
    {
        manager?.ReloadLevel();
    }

    public void EndGame()
    {
        manager?.EndGame();
        Destroy(gameObject);
    }

    public void CheckpointCrossed()
    {
        Debug.Log("checkpoint");
        if (manager != null && manager.NextLevelExists())
        {
            NextLevel();
        }
        else
        {
            state = LevelState.Won;
        } 
    }
}
