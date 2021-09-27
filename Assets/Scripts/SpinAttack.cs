using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttack : MonoBehaviour
{

    public float SpinRange;
    public LayerMask interactable;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spin();
        }
    }
    
    void Spin()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * 2, Quaternion.identity, interactable);

        foreach(Collider hittable in hitColliders)
        {
            Debug.Log("Hit: " + hittable.name);
            hittable.GetComponent<Interactable>().SpinInteract();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale * 2);
    }
}
