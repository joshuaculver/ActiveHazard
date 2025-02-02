using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendAI : MonoBehaviour
{
    private Animator anim;
    public NavMeshAgent agent; 
    
    //DEBUG should be false
    public bool following = true;
    public bool observed = false;

    //Time player needs to be near friend to activate following
    public float proximityTimer;

    //Timer for allowing animation and agent to act
    private float actTimer;
    //Time to wait before moving or acting
    public float actWait = 1f;

    // Start is called before the first frame update
    void Start()
    {
        actTimer = 0f;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        following = false;

        proximityTimer = 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!following)
        {
            //If player proximal
            //proximityTimer -= Time.DeltaTime;
                //If proximityTimer <= 0f;
                //following = true;
        }
        //Need to change player vision colliders so that one can correctly change states
        if(!observed)
        {
            agent.destination = Managers.Player.player.transform.position;
        }

        if(observed)
        {
            anim.speed = 0f;
            actTimer = actWait;

            agent.isStopped = true;
            agent.ResetPath();
            actTimer = actWait;
        }
        else if(actTimer > 0f)
        {
            actTimer -= Time.deltaTime;
        }
        else
        {
            ///Debug.Log("Friend done waiting");
            anim.speed = 1f;
            agent.isStopped = false;
            agent.ResetPath();
        }
    }
}
