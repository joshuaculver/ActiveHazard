using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EyeAgent : MonoBehaviour
{
    public NavMeshAgent agent;

    public List<Transform> nodes;
    //Last node that the agent was moving towards
    private Vector3 lastDest;

    public Light spotLight;

    private bool attacking;
    private float activateTimer;
    public GameObject eyeMesh;

    // Start is called before the first frame update
    void Awake()
    {
        nodes = new List<Transform>();
        agent = GetComponent<NavMeshAgent>();

        nodes = Managers.AI.reqNodes("EYE");

        agent.destination = nodes[0].transform.position;

        int area = NavMesh.GetAreaFromName("EyeArea");
        agent.SetAreaCost(area, 1f);

        agent.Warp(nodes[0].transform.position);

        activateTimer = 3f;
        //getNodes();
    }

    // Update is called once per frame
    void Update()
    {
        EyeLightCheck();

        //TODO DEBUG if
        if(nodes.Count > 0)
        {
            if(!agent.pathPending && agent.remainingDistance < 1f)
            {
                agent.destination = nodes[Random.Range(0, nodes.Count)].transform.position;
            }
            if(AttackCheck())
            {
                attacking = true;
                agent.destination = Managers.Player.player.transform.position;
                //If within proximity to continue chase
                if(agent.remainingDistance <= 3f)
                {
                    activateTimer -= Time.deltaTime;
                    //Move some of this to eye despawn/spawn
                    //MenuManager.Cover(time, color)
                    Managers.Menu.CoverRoutine(1.5f, Color.black);
                    //Stop music
                    Managers.Music.Stop();
                    //Play breaker switch sound
                    Managers.AI.SpawnActive(Managers.AI.FurthestNode(), true);
                    //Check music

                    Managers.AI.DespawnEye();
                }
            }
            //TODO Add cooldown/timer
            else
            {
                if(!agent.pathPending && agent.remainingDistance < 1f)
                {
                    agent.destination = lastDest;
                }
            }
        }
    }

    private void EyeLightCheck()
    {
        RaycastHit hit;
        if(Physics.Raycast(eyeMesh.transform.position, eyeMesh.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Geometry")))
        {
                spotLight.range = Mathf.MoveTowards(
                    spotLight.range,
                    hit.distance * 1f,
                    10f * Time.deltaTime
                );
                spotLight.intensity = Mathf.MoveTowards(
                    spotLight.intensity,
                    hit.distance * 1f,
                    10f * Time.deltaTime
                );
        }
    }

    private bool AttackCheck()
    {
        if(Managers.Player.player != null)
        {   
            //Could make this a collider
            if(Vector3.Distance(transform.position, Managers.Player.transform.position) < 10f)
            {
                Debug.Log("Proximity");

                RaycastHit hit;
                if(Physics.Raycast(eyeMesh.transform.position, Managers.Player.player.transform.position - eyeMesh.transform.position, out hit, Mathf.Infinity))
                {
                    Debug.DrawRay(eyeMesh.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    if(hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("LOS");
                        Debug.DrawRay(eyeMesh.transform.position, eyeMesh.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                        return true;
                    }
                    else
                    {
                        Debug.DrawRay(eyeMesh.transform.position, eyeMesh.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                        Debug.Log("Not player - " + hit.collider.gameObject.name);
                    }
                }
            }
            else
            {
                Debug.Log(Vector3.Distance(transform.position, Managers.Player.transform.position));
            }
        }
        
        return false;
    }
    /*
    public void getNodes()
    {
        //For weird init order stuff. Should request nodes in Awake after spawned by AI manager
        nodes = Managers.AI.reqNodes("EYE");
    }
    */

}
