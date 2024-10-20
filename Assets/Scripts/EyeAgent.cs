using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EyeAgent : MonoBehaviour
{
    public NavMeshAgent agent;

    public List<Transform> nodes;

    public Light spotLight;

    private bool attacking;
    public GameObject eyeMesh;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        nodes = Managers.AI.reqNodes("EYE");

        agent.destination = nodes[0].transform.position;

        int area = NavMesh.GetAreaFromName("EyeArea");
        agent.SetAreaCost(area, 1f);

        agent.Warp(nodes[0].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        EyeLightCheck();

        if(!agent.pathPending && agent.remainingDistance < 1f)
        {
            agent.destination = nodes[Random.Range(0, nodes.Count)].transform.position;
            //agent.destination = Managers.Player.player.transform.position;
        }
        /*
        if(AttackCheck())
        {
            
        }
        */
    }

    private void EyeLightCheck()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Geometry")))
        {
                spotLight.range = Mathf.MoveTowards(
                    spotLight.range,
                    hit.distance * 200f,
                    1000f * Time.deltaTime
                );
                spotLight.intensity = Mathf.MoveTowards(
                    spotLight.intensity,
                    hit.distance * 2000f,
                    1000f * Time.deltaTime
                );
        }
    }

    private bool AttackCheck()
    {
        if(Managers.Player.player != null)
        {   
            //Could make this a collider
            if(Vector3.Distance(transform.position, Managers.Player.transform.position) < 100f)
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
}
