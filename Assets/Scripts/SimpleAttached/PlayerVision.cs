using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVision : MonoBehaviour
{
    public List<Collider> col; 
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Friend")
        {
            //Debug.Log("Friend in player vision collider");
            Managers.AI.friend.observed = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Friend")
        {
            //Debug.Log("Friend left player vision collider");
            Managers.AI.friend.observed = false;
        }
    }
}
