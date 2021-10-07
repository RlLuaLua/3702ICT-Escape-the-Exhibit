using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private void OnDrawGizmos() {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, 0.5f, transform.position.z));
    }
}
