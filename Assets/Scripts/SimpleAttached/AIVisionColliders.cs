using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVisionColliders : MonoBehaviour
{
    //If true only triggers hunt and not chase
    public bool NoticeOnly;

    private void OnTriggerEnter(Collider other)
    {
        if(Managers.AI.active.status != AIStatus.Chase)
        {
            if(other.gameObject.tag == "Player")
            {
                Debug.Log("Player in hazard vision collider");
                if(Managers.AI.active.LOScheck())
                {
                    Debug.Log("Collider + LOS - NoticeOnly: " + NoticeOnly.ToString());
                    if(NoticeOnly && Managers.AI.active.status != AIStatus.Hunt && Managers.AI.active.status != AIStatus.Chase)
                    {
                        Debug.Log("Notice only collider");
                        //False for small/suspicion sting
                        StartCoroutine(Managers.Music.Sting(false));
                        Managers.AI.active.changeState(AIStatus.Hunt);
                    }
                    else
                    {
                        Debug.Log("Chase collider");
                        Managers.AI.active.changeState(AIStatus.Chase);
                    }
                }
            }
        }
    }
}
