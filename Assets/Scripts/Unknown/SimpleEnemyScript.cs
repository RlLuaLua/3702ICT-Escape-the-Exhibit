using UnityEngine;

public class SimpleEnemyScript : Interactable
{
    public override void SpinInteract()
    {
        Destroy(gameObject);
    }
}
