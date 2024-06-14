using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlessAI : AI
{
    void Update()
    {
        agent.destination = Managers.Player.player.transform.position;
    }
}
