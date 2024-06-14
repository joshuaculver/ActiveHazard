using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public List<Transform> nodesA;
    public List<Transform> nodesB;
    public List<Transform> allNodes;
    public GameObject glanceRef;
    public AI active;
    public GameObject actPrefab;
    public Transform activeSpawn;
    public bool spawned = false;
    //Flips for whether danger is rising or falling
    public int danger = 0;
    public bool dangerUp = true;
    private float dangTimer;
    public bool chased = false;
    //15-minutes
    private float demoTimer = 900f;
    public bool titleMode;
    // Start is called before the first frame update
    public void Startup()
    {
        Debug.Log("Dialogue manager starting...");

        GameObject[] newNodes = GameObject.FindGameObjectsWithTag("Node");

        for(int i = 0; i < newNodes.Length;i++)
        {
            Debug.Log("Adding node");
            allNodes.Add(newNodes[i].transform);
        }

        Debug.Log(allNodes);

        //3-minutes
        dangTimer = 180f;

        status = ManagerStatus.Started;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(!spawned && !titleMode)
            {
                danger = 1;
                SpawnActive(FurthestNode().transform);
                spawned = true;
            }
            else
            {
                Destroy(active);
                danger = 0;
                spawned = false;
            }
        }
        //DEBUG
        else if(Input.GetKeyDown(KeyCode.K))
        {
            danger = 11;
            active.killMode = true;
            active.DangerCheck(danger);
        }
        */
        if(dangTimer <= 0)
        {
            if(!spawned && dangerUp && !titleMode)
            {
                danger += 1;
                SpawnActive(FurthestNode().transform);
                active.DangerCheck(danger);
                spawned = true;
            }
            else if(dangerUp)
            {
                if(danger < 10)
                {
                    danger += 1;
                    if(spawned)
                    {
                        active.DangerCheck(danger);
                    }
                    Debug.Log("Danger: " + danger);
                }
            }
            else
            {
                if(danger >= 1)
                {
                    if(danger <= 4)
                    {
                        danger = 4;
                        dangerUp = true;
                    }
                    danger -= 1;
                    if(spawned)
                    {
                        Debug.Log("Danger: " + danger);
                        active.DangerCheck(danger);
                    }
                }
            }
            dangTimer = 80f;
        }
        dangTimer -= Time.deltaTime;
        demoTimer -= Time.deltaTime;
        if(demoTimer <= 0 && !active.killMode)
        {
            danger = 11;
            active.killMode = true;
            active.DangerCheck(danger);
        }
    }

    public List<Transform> reqNodes(string request)
    {
        Debug.Log("Nodes requested...");
        List<Transform> outList = new List<Transform>();
        if(request.Contains("A") || request == "!")
        {
            Debug.Log("A nodes");
            for(int i = 0;i < nodesA.Count();i++)
            {
                outList.Add(nodesA[i]);
            }
        }
        if(request.Contains("B") || request == "!")
        {
            Debug.Log("B nodes");
            for(int i = 0;i < nodesB.Count();i++)
            {
                outList.Add(nodesB[i]);
            }
        }
        return outList;
    }

    public void SpawnActive(Transform pos)
    {
        Debug.Log("Spawning hazard");
        active = Instantiate(actPrefab, pos.position, Quaternion.identity).GetComponent<AI>();
        active.DangerCheck(danger);
    }
    
    //TODO part of AI looking behavior to replace with mathf.moveTowards
    public void moveGlance(Transform center, float dir)
    {
        glanceRef.transform.RotateAround(center.position, Vector3.up, dir * Time.deltaTime);
    }

    //Finds the furthest node from the player that the player doesn't have LOS to
    public Transform FurthestNode()
    {
        if(!titleMode)
        {
            Transform player = Managers.Player.player.transform;
            RaycastHit hit;

            Transform returnNode = nodesA[0];
            for(int i = 0; i < nodesA.Count; i++)
                if(Physics.Raycast(Managers.Player.player.transform.position, nodesA[i].transform.position - Managers.Player.player.transform.position, out hit, Mathf.Infinity))
                {
                    if(Vector3.Distance(Managers.Player.player.transform.position, nodesA[i].transform.position) > hit.distance)
                    {
                        if(Vector3.Distance(Managers.Player.player.transform.position, nodesA[i].transform.position) > Vector3.Distance(Managers.Player.player.transform.position, returnNode.transform.position))
                        {
                            returnNode = nodesA[i];
                        }
                    }
                }

            return returnNode;
        }
        else
        {
            Transform returnNode = active.nodes[Random.Range(1,active.nodes.Count)].transform;
            return returnNode;
        }
    }

}
