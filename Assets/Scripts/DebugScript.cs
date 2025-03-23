using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    void Update()
    {
        if(Managers.AI)
        {
            if(Input.GetKeyUp("H"))
            {
                if(Managers.AI.spawned)
                {
                    Managers.AI.DespawnActive();
                }
                else
                {
                    Managers.AI.SpawnActive(Managers.AI.activeSpawn);
                }
            }
            if(Input.GetKeyUp("J"))
            {
                if(Managers.AI.eyeSpawned)
                {
                    Managers.AI.DespawnEye();
                }
                else
                {
                    Managers.AI.SpawnEye();
                }
            }
        }
    }
}
