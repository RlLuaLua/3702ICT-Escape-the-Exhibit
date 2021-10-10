using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            collider.transform.parent.SetParent(transform);
        }
    }

    void OnTriggerExit(Collider collider) {
            collider.transform.parent.SetParent(null);
    }
}