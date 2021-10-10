using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttack : MonoBehaviour
{
    public AudioSource spin;
    public float SpinRange;
    public LayerMask interactable;
    protected Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("isSpinning", true);
            spin.Play();
            Spin();
        }
    }
    
    void Spin()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale * 2, Quaternion.identity, interactable);

        foreach(Collider hittable in hitColliders)
        {
            Debug.Log("Hit: " + hittable.name);
            hittable.GetComponent<Interactable>().SpinInteract();
        }
    }

    void stopSpinAnimation() {
        animator.SetBool("isSpinning", false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale * 2);
    }
}
