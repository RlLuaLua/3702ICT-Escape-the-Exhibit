using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CheckpointScript : MonoBehaviour
{
    public AudioSource victory;
    public string nextLevelName;
    [SerializeField] bool isEnabled;

    private Renderer meshRenderer;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            victory.Play();
            SceneManager.LoadScene(nextLevelName.Length == 0 ? "Credits" : nextLevelName);
        }
    }

    void Start()
    {
        meshRenderer = gameObject.GetComponent<Renderer>();
    }
    void Update()
    {
        meshRenderer.enabled = (GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
    }
}
