using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateManager : MonoBehaviour, IGameManager
{
    [Serializable]
    public struct Progress
    {
        public Dictionary<string, bool>slideInventory;
        public bool inIntro;
        public bool AEnding;
    }

    [SerializeField]
    public Progress playerProgress = new Progress();

    public ManagerStatus status {get; private set;}

    public GameObject LobbyDoubleA; //Z-161.4
    public GameObject LobbyDoubleB; //Z-159.8
    public GameObject LobbySingle; //Z-145.7

    public GameObject ExitOneA; //155.041
    public GameObject ExitOneB; //-165.839
    public GameObject ExitBlock;
    public GameObject ExitLights;

    public List<GameObject> introBlocks;

    public List<GameObject> slideCollectibles;

    public void Startup()
    {
        Debug.Log("State manager starting...");
        playerProgress.slideInventory = new Dictionary<string, bool>();
        playerProgress.inIntro = true;
        playerProgress.AEnding = false;

        status = ManagerStatus.Started;
        Debug.Log("Intro: " + playerProgress.inIntro);
    }

    public void AddInventory(char set, int id)
    {
        string partA = set.ToString();
        string partB = id.ToString();
        string objectID = partA + partB;
        Debug.Log(playerProgress.slideInventory);
        if(playerProgress.slideInventory.TryAdd(objectID, true))
        {
            Debug.Log("Adding " + objectID + " to inventory");
            StartCoroutine(WaitRemove(1.5f, set, id));
            //CheckInventory(set, id);
        }
    }

    public bool CheckInventory(char set, int id)
    {
        string partA = set.ToString();
        string partB = id.ToString();
        string objectID = partA + partB;
        if(playerProgress.slideInventory.ContainsKey(objectID))
        {
            Debug.Log("Collected confirmed: " + objectID);
            RemoveCollectible(objectID);
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator WaitRemove(float seconds, char set, int id)
    {
        float time = 0f;
        Debug.Log("Waiting...");

        while(time < seconds)
        {
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        CheckInventory(set, id);
        Debug.Log("Removed");
    }

    public void RemoveCollectible(string ID)
    {
        for(int i = 0; i < slideCollectibles.Count; i++)
        {
            if(slideCollectibles[i].GetComponent<SlideCollectible>().GetID() == ID)
            {
                slideCollectibles[i].gameObject.SetActive(false);
                Debug.Log("RemoveCollectible: " + ID.ToString());
                Managers.Slides.EnableNext();
            }
        }
    }

    public void OpenLobby()
    {
        Debug.Log("Opening Lobby");
        LobbyDoubleA.transform.localEulerAngles = new Vector3(0f, 0f, 161.4f); //.448f
        LobbyDoubleB.transform.localEulerAngles = new Vector3(0f, 0f, -159.8f);

        LobbySingle.transform.localEulerAngles = new Vector3(0f, 0f, 145.7f);

        for(int i = 0; i < introBlocks.Count; i++)
        {
            introBlocks[i].gameObject.SetActive(false);
        }

        playerProgress.inIntro = false;

        Managers.Slides.EnableSlideCollectible('A', 1);
        Managers.Slides.EnableSlideCollectible('A', 2);
        Managers.Slides.EnableSlideCollectible('A', 3);
    }

    public void OpenExit()
    {
        Debug.Log("Opening Exit");
        ExitOneA.transform.localEulerAngles = new Vector3(0f, 0f, 155.04f); //.448f
        ExitOneB.transform.localEulerAngles = new Vector3(0f, 0f, -165.839f);

        ExitLights.gameObject.SetActive(true);
        ExitBlock.gameObject.SetActive(false);
    }
}

