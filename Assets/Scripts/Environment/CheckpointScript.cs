using UnityEngine;
using UnityEngine.SceneManagement;
public class CheckpointScript : MonoBehaviour
{
    private LevelManager manager;
    public AudioSource victory;

    private Renderer meshRenderer;

    void Start()
    {
        manager = GameObject.Find("LevelState")?.GetComponent<LevelManager>();   
    }
 
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            manager?.CheckpointCrossed();
            victory.Play();
        }
    }
}
