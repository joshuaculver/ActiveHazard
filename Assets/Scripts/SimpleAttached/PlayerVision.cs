using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Friend")
        {
            Debug.Log("Friend in player vision collider");
            //Set friend to freeze
        }
    }
}
