using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeAreaScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Managers.Player.InEyeArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Managers.Player.InEyeArea = false;
        }
    }
}
