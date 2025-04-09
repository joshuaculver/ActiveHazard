using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeAreaScript : MonoBehaviour
{
    public float SpeedMod;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(Managers.Player.player)
            {
                Managers.Player.ChangeSpeed(SpeedMod);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(Managers.Player.player)
            {
                Managers.Player.ResetSpeed();
            }
        }
    }
}
