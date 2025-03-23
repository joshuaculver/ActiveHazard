using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using System.Collections;
using System.Data.SqlTypes;

public class AI : MonoBehaviour
{
    public NavMeshAgent agent;
    //Default speed which agent resets to if reverting from moveSpeed value
    private float defSpeed = 1.5f;
    //Is multiplied to set agent speed for behaviors
    private float moveSpeed;
    //Multiplies all movement speed
    private float spdMod = 1f;


    public bool busy;
    public Light[] spotLights;
    public LensFlareComponentSRP flare;
    public ParticleSystem particles;

    public timedAudEmitter emit;
    //public Transform[] nodes;
    public List<Transform> nodes;
    public int destNode = 0;

    public AIStatus status;
    private AIStatus lastStatus;
    public GlanceStatus glanceStatus;
    private int looks;
    private float lookTime;
    private float dir;

    private List<int> rndFlip = new List<int>();
    private int flip;

    //Range for rayCast that checks for player
    public float sightRange = 25f;

    //Tracking time since agent has left chase state or "seen player"
    public float contactTime = 0f;
    public Transform lastSeen;
    public bool timing = false;
    public float pursuitDelay = 2f;
    public float pursuitTimer = 0f;
    //How many times the hazard will move to a node or player last known position before leaving hunt behavior 
    private int huntIterations = 0;

    //Attack variables
    public bool attacking = false;
    private float atkTimer = 0f;
    private float atkLen = 5f;
    private int warnings = 2;
    private int warningsDefault = 2;
    public bool ignorePlayer = false;

    private bool chased = false;

    public GameObject debugMarker;

    //When true forces the hazard to stay in hunt behavior until contact is made with the player
    public bool hardHunt = false;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        flare = spotLights[0].GetComponent<LensFlareComponentSRP>();
        //Gets the region of nodes to start patrolling

        nodes = Managers.AI.reqNodes("HAZARD");

        Debug.Log("Status: " + status);
        destNode = 0;
        rndFlip.Add(-1);
        rndFlip.Add(1);

        Debug.Log("Done");

        busy = false;
        if (status == AIStatus.Avoid)
        {
            lightSwitch(false);
        }
        else
        {
            lightSwitch(true);
        }

        moveSpeed = defSpeed;

        //debugMarker = Instantiate(debugMarker, new Vector3(0f, 0f, 0f), Quaternion.identity);
    }

    //Move update actions to functions. Create busy variable to cause return during udpate if new action isn't wanted
    void Update()
    {
        //debugMarker.transform.position = agent.destination;
        lightDistCheck();
        //Checking if agent is moving to turn sound emitter on or off
        if(!Mathf.Approximately(agent.velocity.x, 0) || !Mathf.Approximately(agent.velocity.y, 0))
            {
                emit.running = true;
            }
        else
            {
                emit.running = false;
            }

        if(timing)
        {
            contactTime += Time.deltaTime;
            pursuitTimer += Time.deltaTime;
        }

        //Normal behavior
        if(busy)
        {
            return;
        }
        else
        {
            //Main split for giving agent target for pathing
            if(status == AIStatus.Chase || status == AIStatus.Pursue)
            {
                //"Attacking behavior
                if(playerRay())
                {
                    LookTarget(Managers.Player.player.transform.position, 1f);
                    
                    
                    if(agent.remainingDistance <= 6f)
                    {
                        //Debug.Log("In range");
                        Attack();
                    }
                    else
                    {
                        attacking = false;
                    }
                }
                else
                {
                    LookTarget(Managers.Player.player.transform.position, .2f);
                }
                ChaseBehav();
            }
            else if(!agent.pathPending && agent.remainingDistance < 1f)
            {
                //chance that when wandering or hunting AI stops to look around
                if(status != AIStatus.Glance && status != AIStatus.Avoid && Random.Range(-4,Managers.AI.danger) == 1)
                {
                    changeState(AIStatus.Glance);
                }
                else
                {
                    PathBehav();
                }
            }
        }

    }

    int Flip()
    {
        int num = rndFlip.IndexOf(Random.Range(0,2));
        flip = num;
        return rndFlip.IndexOf(Random.Range(0,2));
    }

    //Handles switching behavior states
    //TODO see if multiple states can be handled the same way i.e. if(status == AIStatus.Hunt || status == AIStatus.Glace)
    public void changeState(AIStatus newState)
    {
        Debug.Log("Changing AI status");
        if(newState == status)
        {
            return;
        }

        if(newState == AIStatus.Wander)
        {
            Debug.Log("Changing AI status: Wander");
            if(status == AIStatus.Hunt)
            {
                agent.ResetPath();
                status = AIStatus.Wander;

                Managers.Player.canInteract(true);
            }
            else if(status == AIStatus.Pursue)
            {
                status = AIStatus.Hunt;

                Managers.Player.canInteract(true);
            }
            else if(status == AIStatus.Avoid)
            {
                Managers.AI.DespawnActive();
                //lightSwitch(true);
                status = AIStatus.Wander;
                Managers.Player.canInteract(true);
            }
            else if(status == AIStatus.Glance)
            {
                status = AIStatus.Wander;
                Managers.Player.canInteract(true);
            }
            else
            {
                Debug.Log("Cannot switch from " + status + " -> " + newState);
            }
        }
        else if(newState == AIStatus.Avoid)
        {
            Debug.Log("Changing AI status: Avoid");
            if(status == AIStatus.Wander)
            {
                agent.ResetPath();
                lightSwitch(false);
                status = AIStatus.Avoid;
                emit.setVol(Managers.Music.defaultFXVol / 2);

                Managers.Player.canInteract(true);
            }
            else if(status == AIStatus.Glance)
            {
                lightSwitch(false);
                status = AIStatus.Avoid;
                emit.setVol(Managers.Music.defaultFXVol / 2);

                Managers.Player.canInteract(true);
            }
            else
            {
                Debug.Log("Cannot switch from " + status + " -> " + newState);
            }

        }
        else if(newState == AIStatus.Hunt)
        {
            Debug.Log("Changing AI status: Hunt");
            if(status == AIStatus.Wander || status == AIStatus.Glance || status == AIStatus.Pursue)
            {
                huntIterations = Random.Range(2, 5);
                timing = true;
                contactTime = 0;
                status = AIStatus.Hunt;

                if(lastSeen == null)
                {
                    lastSeen = Managers.Player.player.transform;
                }
                agent.ResetPath();
                GoHunt();

                Managers.Player.canInteract(true);
            }
            else
            {
                Debug.Log("Cannot switch from " + status + " -> " + newState);
            }
        }
        else if(newState == AIStatus.Pursue)
        {
            if(status == AIStatus.Chase)
            {
                attacking = false;
                timing = true;
                contactTime = 0;
                agent.stoppingDistance = 0f;
                pursuitTimer = 0;
                status = AIStatus.Pursue;

                Managers.Player.canInteract(false);
            }
            else if(status == AIStatus.Hunt)
            {
                timing = true;
                contactTime = 0;
                status = AIStatus.Pursue;

                Managers.Player.canInteract(false);
            }
            else
            {
                Debug.Log("Cannot switch from " + status + " -> " + newState);
            }
        }
        //Switch to chase. Only state which should always be acccesible
        else if(newState == AIStatus.Chase)
        {
            agent.stoppingDistance = 6f;
            timing = true;
            contactTime = 0;
            agent.ResetPath();
            status = AIStatus.Chase;
            Managers.AI.chased = true;

            //True for full sting
            StartCoroutine(Managers.Music.Sting(true));
            Managers.Player.canInteract(false);

            //Contact was made so forced hunt can end
            if(hardHunt)
            {
                spdMod = 1f;
                hardHunt = false;
            }

        }
        else if(newState == AIStatus.Glance)
        {
            Debug.Log("Changing AI status: Glance");
            if(status == AIStatus.Wander)
            {
                lastStatus = AIStatus.Wander;
                status = AIStatus.Glance;

                Managers.AI.glanceRef.transform.position = nodes[Random.Range(0,nodes.Count)].transform.position;

                looks = Random.Range(1, 6);
                lookTime = Random.Range(1f, 4f);
                dir = Random.Range(3f,7f) * Flip();
            }
            else if(status == AIStatus.Hunt)
            {
                lastStatus = AIStatus.Hunt;
                status = AIStatus.Glance;

                Managers.AI.glanceRef.transform.position = lastSeen.position;
                looks = Random.Range(1, 4);
                lookTime = Random.Range(1f, 2f);
                dir = Random.Range(4f,8f) * Flip();
            }
            else
            {
                Debug.Log("Cannot switch from " + status + " -> " + newState);
            }
        }

        Managers.Music.check();
        
        Debug.Log("State:" + status);
    }

    //TODO handle pausing timer/state changes/cool down when not in chase
    void Attack()
    {
        attacking = true;
        if(atkTimer >= atkLen)
        {
            if(warnings > 0)
            {
                warnings -= 1;
                atkTimer = 0f;
                StartCoroutine(MuzzFlash());
                Managers.Music.atkUpdate(warnings);
            }
            else if(warnings == 0)
            {
                warnings -= 1;
                atkTimer = 0f;
                StartCoroutine(MuzzFlash());
                //TODO move to function
                Debug.Log("Game over :)");
                Managers.Player.Die();
                lightShadow(false);
                busy = true;
            }
            atkTimer = 0f;
        }
        else
        {
            atkTimer += Time.deltaTime;
            //Debug.Log("Aiming: " + atkTimer);
        }
    }

    //Pathing and behavior for when agent is chasing the player
    void ChaseBehav()
    {
        if(status == AIStatus.Chase)
        {
            //Debug.Log("Entering chase behavior");
            agent.stoppingDistance = 5f;
            if(!playerRay() || agent.remainingDistance > 20f)
            {
                //Debug.Log("LOS broken or max distance exceeded");
                changeState(AIStatus.Pursue);
            }
            else if(agent.remainingDistance > 4.5f)
            {
                //Debug.Log("Closing distance");
                GoChase();
            }
            else if(!playerRay())
            {
                GoChase();
            }
            else if(agent.remainingDistance <= 5f && playerRay())
            {
                //Debug.Log("In range");
                GoChase();
            }
            else if(contactTime > 5f)
            {
                changeState(AIStatus.Pursue);
            }
        }
        else if(status == AIStatus.Pursue)
        {
            if(playerRay())
            {
                changeState(AIStatus.Chase);
            }
            else if(contactTime >= 15f)
            {
                lastSeen = Managers.Player.player.transform;
                changeState(AIStatus.Hunt);
            }
            else
            {
                GoPursue();
            }
        }
    }

    //Behavior for when the agent is not currently chasing the player
    void PathBehav()
    {
        agent.stoppingDistance = 0f;
        //Checks if there's a path being calculated and if the agent is close to target
        if(status == AIStatus.Glance)
        {
            LookGlance();
        }
        else if(status == AIStatus.Avoid && (Vector3.Distance(Managers.Player.player.transform.position, agent.transform.position) < 17f))
        {
            Debug.Log("Player too close");
            agent.isStopped = true;
            GoAvoid();
        }
        else if(!agent.pathPending && agent.remainingDistance < 1f)
        {
            if(status == AIStatus.Wander || status == AIStatus.Avoid)
                {
                    Debug.Log("Reached destination");
                    GoWander();
                }
            else if(status == AIStatus.Hunt)
            {   
                if(agent.remainingDistance < 1f)
                {
                    agent.isStopped = true;
                    GoHunt();
                }
            }
        }
    }


    //Attempts to maintain distance from player
    //TODO switch from transform.position to NavMesh.CalculatePath. Agent can get cornered due choosing "away" in terms of distance instead of pathing distance
    void GoAvoid()
    {
        Debug.Log("Avoiding");
        Transform newDest = agent.transform;
        List<Transform> tempNodes = new List<Transform>();
        for (int i = 0; i < nodes.Count; i++)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, nodes[i].position, NavMesh.AllAreas, path);

            if(Vector3.Distance(path.corners[0], Managers.Player.player.transform.position) > Vector3.Distance(path.corners[0], transform.position))
            {
                tempNodes.Add(nodes[i]);
            }
        }
        if(tempNodes.Count == 0)
        {
            Debug.Log("Failed to count nodes");
        }
        else
        {
            //TODO make a less stinky way to sort
            //TODO agent currently looks for furthest node from player, when they should look for node furthest from player or shortest path distance
            newDest = tempNodes[0];
            for (int i = 0; i < tempNodes.Count; i++)
            {
                //if (Vector3.Distance(tempNodes[i].position, agent.transform.position) < Vector3.Distance(newDest.position, agent.transform.position))
                if(Vector3.Distance(tempNodes[i].position, Managers.Player.player.transform.position) > Vector3.Distance(newDest.position, Managers.Player.player.transform.position))
                {
                    newDest = tempNodes[i];
                }
            }
        }

        agent.destination = newDest.position;
        //agent.speed = 4f;
        agent.speed = moveSpeed * 2.67f * spdMod;

        agent.isStopped = false;
    }

    //General behavior where agent randomly picks nodes from node list to path to
    //TODO make sure attack timer stops when leaving chase
    void GoWander()
    {
        if (nodes.Count <= 0)
        {
            return;
        }
        else if(!agent.pathPending)
        {
            
            int currNode = destNode;
            while(currNode == destNode)
            {
                //One in ten to pick a random node. Otherwise advance through nodes sequentially.
                if(Random.Range(1,11) == 4)
                {  
                    destNode = Random.Range(0, nodes.Count);
                    Debug.Log("Wandering to random node: " + destNode);
                }
                else
                {
                    destNode = (destNode + flip) % nodes.Count;
                    destNode = Mathf.Abs(destNode);
                    Debug.Log("Wandering to next Node: " + destNode);
                }
            }
            if(Random.Range(1, 21) == 1)
            {
                Debug.Log("Going fast randomly");
                //agent.speed = Random.Range(3.5f, 5f);
                agent.speed = Random.Range(moveSpeed * 2.4f, moveSpeed * 3.4f) * spdMod;
            }
            else
            {
                //agent.speed = Random.Range(0.75f, 3.5f);
                agent.speed = Random.Range(moveSpeed * 0.5f, moveSpeed * 2.4f) * spdMod;
            }
            //agent.speed = 3;
            agent.destination = nodes[destNode].position;
            
            Debug.Log("At " + nodes[destNode].position);
        }
    }

    //Agent is given player's current position to path to. Agent path does not udpate until they reach position
    void GoHunt()
    {
        if(!agent.pathPending)
        {
            Debug.Log("Hunting");
            //agent.speed = Random.Range(1.5f, 3.25f);
            agent.speed = Random.Range(moveSpeed, moveSpeed * 2.2f) * spdMod;
            //agent.destination = Managers.Player.player.transform.position; //+ new Vector3(Random.Range(-4.0f, 4.0f), 0.0f, Random.Range(-4.0f, 4.0f));
            if(playerRay() || Vector3.Distance(transform.position, Managers.Player.player.transform.position) <= 5)
            {
                Debug.Log("Hunt: Either LOS or <= 5 distance");
               agent.destination = Managers.Player.player.transform.position; 
            }
            else if(Managers.Player.nearNodes.Count > 0)
            {
                Debug.Log("Hunt: Dequeing near node");
                agent.destination = Managers.Player.nearNodes.Dequeue().position;
            }
            else
            {
                Debug.Log("Hunt: near nodes empty");
                agent.destination = Managers.Player.player.transform.position;         
            }
            agent.isStopped = false;

            huntIterations -= 1;
        }

        //TODO figure out more interesting way to handle this
        /*
        if(huntIterations <= 0)
        {
            Debug.Log("Lost player");
            timing = false;
            changeState(AIStatus.Wander);
            agent.ResetPath();
            if(chased)
            {
                chased = false;
                Managers.AI.dangerUp = false;
            }
        }
        */
        if(huntIterations <= 0)
        {
            if(hardHunt)
            {
                huntIterations = Random.Range(2, 5);
                spdMod += Random.Range(moveSpeed * 0.03f, moveSpeed * 0.06f);
            }
            else
            {
                Debug.Log("Lost player");
                timing = false;
                changeState(AIStatus.Wander);
                agent.ResetPath();
                if(chased)
                {
                    chased = false;
                    Managers.AI.dangerUp = false;
                }
            }
        }
    }

    //Pathing for agent while player is near and agent has LOS
    void GoChase()
    {
        chased = true;
        //Debug.Log("Chasing");
        //agent.speed = 2.25f;
        agent.speed = moveSpeed * 1.5f * spdMod;
        if(playerRay())
        {
            //Debug.Log("LOS");
            agent.destination = Managers.Player.player.transform.position;
        }
        else
        {
            //Debug.Log("LOS broken");
            changeState(AIStatus.Pursue);
        }
    }

    void GoPursue()
    {
        //agent.speed = 2.15f;
        agent.speed = moveSpeed * 1.44f * spdMod;
        //Debug.Log("Updating destination in pursue");
        if(pursuitTimer >= pursuitDelay)
        {
            agent.destination = Managers.Player.player.transform.position;
            pursuitTimer = 0;
        }
    }

    //Rotates agent towards player. Used when pathing wouldn't orient agent towards player
    //TODO replace Slerp with mathf.moveTowards()
    void LookTarget(Vector3 targPos, float spd)
    {
        Vector3 pos = targPos - transform.position;
        pos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(pos);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, spd);
    }

    void LookGlance()
    {
        if(looks > 0)
        {
            if(lookTime > 0f)
            {
                LookTarget(Managers.AI.glanceRef.transform.position, 0.001f);
                Managers.AI.moveGlance(transform, dir);
                lookTime -= Time.deltaTime;
            }
            else
            {
                looks -= 1;
                lookTime = Random.Range(1f, 3f);
                dir = Random.Range(3f, 6f) * Flip();
                Debug.Log("Next look, left: " + looks);
            }
        }
        else
        {
            Debug.Log("Done Glancing");
            changeState(lastStatus);
        }
    }

    //While in chase behavior casts a ray towards the player. In any other behavior casts ray infront of agent
    public bool playerRay()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position , transform.TransformDirection(Vector3.forward) * 25f, Color.blue);

        if(status == AIStatus.Chase || status == AIStatus.Pursue)
        {
            if(Physics.Raycast(transform.position, Managers.Player.player.transform.position - transform.position, out hit, sightRange))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("Player in range and LOS");
                    return true;
                }
            }
        }
        else
        {
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, sightRange))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("Spotted player");
                    return true;
                }
            }
        }

        return false;
    }

    public bool LOScheck()
    {
        Debug.Log("Hazard LOS check");
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Managers.Player.player.transform.position - transform.position, out hit, Mathf.Infinity))
        {
            if(hit.collider.CompareTag("Player"))
            {
                Debug.Log("Has LOS");
                return true;
            }
            else
            {
                Debug.Log("Not player - " + hit.collider.gameObject.name);
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void lightDistCheck()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Geometry")))
        {
            if(hit.distance < 50f)
            {
                
                spotLights[0].range = Mathf.MoveTowards(
                    spotLights[0].range,
                    hit.distance + 2f,
                    40f * Time.deltaTime
                );
            }
            else
            {
                spotLights[0].range = Mathf.MoveTowards(
                    spotLights[0].range,
                    50f,
                    40f * Time.deltaTime
                );
            }
        }
    }

    void lightSwitch(bool set)
    {
        flare.enabled = set;
        if(set)
        {
            particles.Play();
        }
        else
        {
            particles.Stop();
        }
        for(int i = 0; i < spotLights.Length; i++)
        {
            spotLights[i].enabled = set;
        }
    }

    void lightShadow(bool set)
    {
        Debug.Log("Changing hazard spotlight shadows");
        for(int i = 0; i < spotLights.Length; i++)
        {
            if(set)
            {
                Debug.Log("Turning on shadows on hazard spotlight");
                spotLights[i].shadows = LightShadows.Hard;
            }
            else
            {
                Debug.Log("Turning off shadows on hazard spotlight");
                spotLights[i].shadows = LightShadows.None;
            }
        }
    }

    private IEnumerator MuzzFlash()
    {
        Color flash = new Color(1f, 0.8f, 0.5f, 1f);
        float timer = 0f;
        while(timer < 0.1f)
        {
            spotLights[0].enabled = false;
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        
        spotLights[0].color = flash;
        spotLights[0].intensity = 75f;
        spotLights[0].innerSpotAngle = 0f;
        spotLights[0].spotAngle = 170f;
        spotLights[0].enabled = true;
        emit.emitBang();
        Managers.Player.Attacked();

        while(timer < 0.125f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;

        spotLights[0].enabled = false;
        spotLights[0].color = Color.white;
        spotLights[0].intensity = 50f;
        spotLights[0].innerSpotAngle = 30f;
        spotLights[0].spotAngle = 100f;
        spotLights[0].enabled = true;
        

        while(timer < 0.2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        spotLights[0].enabled = true;
        

        yield break;
    }
}
