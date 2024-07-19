using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCubeMarker : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Color col = new Color(0f, 255f, 0f, .25f);
        Gizmos.color = col;
        Gizmos.DrawSphere(transform.position, 1.5f);
    }    
}
