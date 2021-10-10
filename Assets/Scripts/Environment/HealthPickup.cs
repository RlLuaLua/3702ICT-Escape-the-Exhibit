using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healthAmount = 1f;
    public AudioSource pickup;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<HealthController>().AddHealth(healthAmount);
            Destroy(gameObject);
            pickup.Play();
        }
    }
}

