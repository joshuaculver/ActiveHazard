using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMovement : MonoBehaviour
{
    public Vector3 offset;
    public bool player;

    public Transform target;
    public Transform rotateTarget;

    // Update is called once per frame
    void Update()
    {
        if(player)
        {
            if(Managers.Player.player != null)
            {
                transform.position = Managers.Player.cam.transform.position + offset;

                if(rotateTarget != null)
                {
                    transform.rotation = Managers.Player.cam.transform.rotation;
                }
            }
        }
        else
        {
            transform.position = target.transform.position + offset;

            if(rotateTarget != null)
            {
                transform.rotation = rotateTarget.transform.rotation;
            }
        }
    }
}
