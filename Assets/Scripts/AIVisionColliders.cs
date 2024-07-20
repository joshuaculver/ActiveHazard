using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVisionColliders : MonoBehaviour
{
    //If true only triggers hunt and not chase
    public bool NoticeOnly;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(Managers.AI.active.LOScheck())
            {
                if(NoticeOnly)
                {
                    Managers.AI.active.changeState(AIStatus.Hunt);
                }
                else
                {
                    Managers.AI.active.changeState(AIStatus.Chase);
                }
            }
        }
    }
}
