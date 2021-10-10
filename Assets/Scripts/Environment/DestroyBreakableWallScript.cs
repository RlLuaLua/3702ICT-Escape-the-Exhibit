using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBreakableWallScript : MonoBehaviour
{
    public ParticleSystem particles;
    public GameObject brokenFence;

    public void DestroyAndSpawnEmitter()
    {
        var instantiatedParticles = Instantiate(particles, transform.position, transform.rotation);
        var instantiatedBrokenFence = Instantiate(brokenFence, transform.position, transform.rotation);
        
        Destroy(gameObject);
        Destroy(instantiatedParticles, particles.main.startLifetime.constant);
        Destroy(instantiatedBrokenFence, particles.main.startLifetime.constant);
    }
}
