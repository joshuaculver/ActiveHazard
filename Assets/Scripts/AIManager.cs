using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public List<Transform> nodesA;
    public List<Transform> nodesB;
    public List<Transform> nodesC;
    public List<Transform> allNodes;
    public GameObject glanceRef;
    public AI active;
    public GameObject actPrefab;
    public Transform activeSpawn;
    public bool spawned = false;
    //Flips for whether danger is rising or falling
    public int danger;
    public bool dangerUp = true;
    private float dangTimer;
    public bool chased = false;
    

    // Start is called before the first frame update
    public void Startup()
    {
        Debug.Log("AI manager starting...");

        GameObject[] newNodes = GameObject.FindGameObjectsWithTag("Node");

        for(int i = 0; i < newNodes.Length;i++)
        {
            Debug.Log("Adding node: " + newNodes[i].name);
            allNodes.Add(newNodes[i].transform);
        }

        allNodes.Sort((x, y) => x.name.CompareTo(y.name));

        Debug.Log(allNodes);

        //3-minutes
        dangTimer = 180f;
        danger = 3;

        status = ManagerStatus.Started;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(!spawned)
            {
                //danger = 1;
                SpawnActive(FurthestNode().transform);
                spawned = true;
            }
            else
            {
                Destroy(active);
                //danger = 0;
                spawned = false;
            }
        }
        if(dangTimer <= 0)
        {
            if(!spawned && dangerUp)
            {
                danger += 1;
                SpawnActive(FurthestNode().transform);
                DangerCheck();
                spawned = true;
            }
            else if(dangerUp)
            {
                if(danger < 10)
                {
                    danger += 1;
                    if(spawned)
                    {
                        DangerCheck();
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
                        DangerCheck();
                    }
                }
            }
            dangTimer = 80f;
        }
        dangTimer -= Time.deltaTime;
    }

    public void DangerCheck()
    {
        if(active.status != AIStatus.Chase && active.status != AIStatus.Pursue)
        {
            if(danger == 0 || danger > 10)
            {
                return;
            }

            if(danger == 10)
            {
                Debug.Log("Danger check: 10");
                active.changeState(AIStatus.Pursue);
            }
            else if(danger > 6 && danger <= 9)
            {
                Debug.Log("Danger check: switch to hunt");
                active.changeState(AIStatus.Hunt);
            }
            else if(danger > 2 && danger <= 6)
            {
                Debug.Log("Danger check: switch to wander");
                active.changeState(AIStatus.Wander);
            }
            else if(danger <= 2)
            {
                Debug.Log("Danger check: switch to avoid");
                active.changeState(AIStatus.Avoid);
            }
        }
    }

    public List<Transform> reqNodes(string request)
    {
        Debug.Log("Nodes requested...");
        List<Transform> outList = new List<Transform>();
        /*
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
        */
        outList = allNodes;
        return outList;
    }

    public void SpawnActive(Transform pos)
    {
        Debug.Log("Spawning hazard");
        active = Instantiate(actPrefab, pos.position, Quaternion.identity).GetComponent<AI>();
        DangerCheck();
    }
    
    //TODO part of AI looking behavior to replace with mathf.moveTowards
    public void moveGlance(Transform center, float dir)
    {
        glanceRef.transform.RotateAround(center.position, Vector3.up, dir * Time.deltaTime);
    }

    //Finds the furthest node from the player that the player doesn't have LOS to
    public Transform FurthestNode()
    {
        Transform player = Managers.Player.player.transform;
        RaycastHit hit;

        Transform returnNode = allNodes[0];
        for(int i = 0; i < allNodes.Count; i++)
            if(Physics.Raycast(Managers.Player.player.transform.position, allNodes[i].transform.position - Managers.Player.player.transform.position, out hit, Mathf.Infinity))
            {
                if(Vector3.Distance(Managers.Player.player.transform.position, allNodes[i].transform.position) > hit.distance)
                {
                    if(Vector3.Distance(Managers.Player.player.transform.position, allNodes[i].transform.position) > Vector3.Distance(Managers.Player.player.transform.position, returnNode.transform.position))
                    {
                        returnNode = allNodes[i];
                    }
                }
            }

        return returnNode;
    }
}
