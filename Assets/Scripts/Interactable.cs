using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 1f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public virtual void Interact()
    {
        //To be overriden 
    }
}
