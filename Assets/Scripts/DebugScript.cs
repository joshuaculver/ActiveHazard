using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    void Update()
    {
        if(Managers.AI)
        {
            if(Input.GetKeyUp(KeyCode.U))
            {
                if(Managers.AI.spawned)
                {
                    Debug.Log("Spawning Hazard");
                    Managers.AI.DespawnActive();
                }
                else
                {
                    Debug.Log("Despawning Hazard");
                    Managers.AI.SpawnActive(Managers.AI.activeSpawn);
                }
            }
            if(Input.GetKeyUp(KeyCode.I))
            {
                if(Managers.AI.eyeSpawned)
                {
                    Debug.Log("Spawning Eye");
                    Managers.AI.DespawnEye();
                }
                else
                {
                    Debug.Log("Despawning Eye");
                    Managers.AI.SpawnEye();
                }
            }
        }
    }
}
