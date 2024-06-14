using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(NameofManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(AIManager))]
[RequireComponent(typeof(MusicManager))]

public class Managers : MonoBehaviour
{
    public static PlayerManager Player {get; private set;}
    public static AIManager AI {get; private set;}
    public static MusicManager Music {get; private set;}

    private List<IGameManager> _startSequence;

    void Awake()
    {
        Player = GetComponent<PlayerManager>();
        AI = GetComponent<AIManager>();
        Music = GetComponent<MusicManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(AI);
        _startSequence.Add(Music);

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers()
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    numReady++;
                }
            }

            if (numReady > lastReady)
                Debug.Log("Progress: " + numReady + "/" + numModules);

            yield return null;
        }

        Debug.Log("All managers started");
    }
}
