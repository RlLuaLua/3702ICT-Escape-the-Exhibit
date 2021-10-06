using UnityEngine;
using UnityEngine.UI;

public class LevelGUIManager : MonoBehaviour
{
    [SerializeField] private Image paintingImage;
    [SerializeField] private Image healthMask;
    [SerializeField] private Text enemiesTextObject;

    private HealthController playerHealthController;
    private GameObject[] enemies;
    private GameObject[] images;

    private int aliveEnemies;
    private float healthMaskMaxWitdth;
    private float pictureSpawnInset = 100.0f;

    void Start()
    {
        playerHealthController = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        images = new GameObject[enemies.Length];
        
        healthMaskMaxWitdth = healthMask.rectTransform.sizeDelta.x;

        Vector3[] paintingBounds = new Vector3[4];
        paintingImage.rectTransform.GetLocalCorners(paintingBounds);

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
        aliveEnemies = enemies.Length;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) {
                Destroy(images[i]);
                aliveEnemies--;
            }
        }

        enemiesTextObject.text = "Enemies: " + aliveEnemies;
        healthMask.rectTransform.sizeDelta = new Vector2(healthMaskMaxWitdth / playerHealthController.max_health * playerHealthController.cur_health, healthMask.rectTransform.sizeDelta.y);
    }
}
