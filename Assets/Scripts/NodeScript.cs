using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    public bool eye;
    void OnDrawGizmos()
    {
        Color col = new Color(0f, 255f, 0f, .25f);
        if(eye)
        {
            col = new Color(125f, 125f, 0f, .25f);
        }
        Gizmos.color = col;
        Gizmos.DrawSphere(transform.position, 1.5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Managers.Player.nearNodes.Enqueue(transform);
            if(Managers.Player.nearNodes.Count > 3)
            {
                Managers.Player.nearNodes.Dequeue();
            }
        }
    }
}
